﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* IUpdater implementation for the locally controlled player.
*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
public class LocalPlayerUpdater : MonoBehaviour, IUpdater {
	/* Accessors. */
	public PhotonPlayer Owner {get; private set;}
	public PhotonView View {get; private set;}
	public int ViewID
	{
		get
		{
			return View.viewID;
		}
		private set
		{
			View.viewID = value;
		}
	}

	/* Used for recording chagnes to states between calls to FixedUpdate. */
	Vector2 positionAtPreviousFrame; // records player position at previous call to FixedUpdate
	float rotationAtPreviuosFrame; // records object rotation at previous call to FixedUpdate
	// timestamped list of all state changes since latest server update
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();

	/* Fields used for server reconciliation */
	/* The body double rigidbody is used when client prediction is enabled:
	* with client prediction, the client applies movement to itself immediately, and stores the changes that occur
	* in-between calls to FixedUpdate. These changes are then re-applied to the position reported by the server
	* in order to extrapolate the up-to-date server position. The rigidbody is required to continue simulating
	* client input on the reported server position (so that it remains in-sync with client input until the next
	* server update is received).
	*/
	public GameObject playerBodyDouble;
	Rigidbody2D bodyDouble;

	/*
	* State we start merging from. Note that these are updated each call to FixedUpdate if prediction is enabled.
	* Otherwise, they are set each server update.
	*/
	Vector2 moveFrom; // client position before merge
	float rotateFrom; // client rotation before merge

	/* State we are merging to. These states are given by the server and are authoritative over player states. */
	Vector2 updatePosition; // position reported by server
	Vector2 updateVelocity; // velocity reported by server - only used when client prediction is off
	float updateRotation;   // rotation reported by server

	/* State change incurred by the state merge.
	* We record them as we need to cancel them out when recording state changes due to client movement.
	* That is, these are only used for client prediction.
	*/
	Vector2 lerpMove;
	float lerpRotate;

	/* Fields related to state merging. */
	double previousUpdateTS; // timestamp of previous server update
	double currentSynchDuration = 0; // time the current merge has been running for - 0 at class creation
	double totalSynchDuration; // total time the current merge should run for

	/* Fields used for recomputing the value of totalSynchDuration. */
	// These are used when client prediction is off.
	public float updateTSDeltaWeight = 0.2f; // weight attributed to previous delay between updates
	public float synchTimePadding = 0.1f; // pads total synch duration for smoother movement
	// These are used when client prediction is on.
	// Same use as above, but used when prediction is enabled.
	public float predictionUpdateTSDeltaWeight = 0.75f;
	public float prdictionSynchTimePadding = 0.2f;

	public bool useClientPrediction = true; // guess what this does =D

	/* Fields used to determine whether input axes changed since last call to FixedUpdate. */
	/* New input values necessary for server synchronisation should be declared here. */
	float strafeInput;
	float thrustInput;
	float torqueInput;

	/* Movement-related modifiers */
	public float strafeModifier = 100;
	public float thrustModifier = 100;
	public float torqueModifier = 1500;

	/* Rigidbody ref for quick reference. */
	Rigidbody2D rb;

	/**
	* Simple Awake method.
	*/
	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		View = GetComponent<PhotonView> ();

		positionAtPreviousFrame = rb.position;
		rotationAtPreviuosFrame = rb.rotation;

		// Technically correct set, as we initialise the object when instructed by the server
		previousUpdateTS = PhotonNetwork.time;
		// Initialised to maximum send-rate, approximating the worst-case update rate at creation
		totalSynchDuration = 1 / PhotonNetwork.sendRateOnSerialize;
	}


	/**
	* Setup function called after initialising the object to set ownership and synch information.
	* Also creates a rigidbody bodyDouble (see above for explanation of how it is used)
	*/
	public void SetupSpawn (PhotonPlayer owner, int viewID)
	{
		Owner = owner;
		ViewID = viewID;
		bodyDouble = (Instantiate(playerBodyDouble, rb.position, Quaternion.identity) as GameObject).GetComponent<Rigidbody2D> ();
		bodyDouble.rotation = rb.rotation;
	}


	/**
	* Fetches input related to movement and updates the client and server model positions.
	*/
	void FixedUpdate ()
	{
		if (!Owner.isLocal)
		{
			Debug.LogError ("Player script attached to non-player object");
			return;
		}
		// get new inputs from input manager
		float thrust = InputManager.Instance.ThrustAxis;
		float strafe = InputManager.Instance.StrafeAxis;
		float torque = InputManager.Instance.TorqueAxis;

		// check if new inputs are different from old inputs
		CheckInputChange (thrust, strafe, torque);

		// update synch duration
		currentSynchDuration += Time.fixedDeltaTime;

		// moves this object
		UpdateModelState ();
	}

	/**
	* Checks if given inputs are different from previous inputs.
	* If they are, sends an update RPC to the server and updates the local inputs to their new value.
	*/
	void CheckInputChange (float thrust, float strafe, float torque)
	{
		// check if inputs changed since last call to FixedUpdate and send update to server if true
		if (strafe != strafeInput || thrust != thrustInput || torque != torqueInput)
		{
			Debug.Log ("Sending position update to server");
			View.RPC ("UpdateInput", PhotonTargets.MasterClient, strafe, thrust, torque);
			thrustInput = thrust;
			strafeInput = strafe;
			torqueInput = torque;
		}
	}

	/**
	* Updates client model position.
	* If client prediction is in use, records state changes and applies input directly to the client and server models,
	* then moves client towards server position.
	* If prediction is off, moves client towards the position reported by the server w/o applying input directly.
	*/
	void UpdateModelState ()
	{
		if (useClientPrediction)
		{
			// record state changes and 
			RecordStateChanges ();
			moveFrom = rb.position;
			rotateFrom = rb.rotation;

			// compute movement forces as a function of input
			Vector2 moveForce = new Vector2 (strafeInput,thrustInput).normalized;
			moveForce = new Vector2(moveForce.x * strafeModifier * Time.fixedDeltaTime, moveForce.y * thrustModifier * Time.fixedDeltaTime);
			float torqueValue = torqueInput * torqueModifier * Time.fixedDeltaTime;

			// apply forces to client model
			rb.AddForce (moveForce);
			rb.AddTorque (torqueValue);
			// apply forces to server model
			bodyDouble.AddForce (moveForce);
			bodyDouble.AddTorque (torqueValue);
		}

		// Lerping is used to 'seamlessly' merge client and server position over time
		rb.position = Vector2.Lerp (moveFrom, bodyDouble.position, (float)(currentSynchDuration/totalSynchDuration));
		lerpMove = rb.position - moveFrom;

		rb.rotation = Mathf.Lerp (rotateFrom, bodyDouble.rotation, (float)(currentSynchDuration/totalSynchDuration));
		lerpRotate = rb.rotation - rotateFrom;
	}

	/**
	* Checks if the client's model's state changed since last call.
	* If it has, recods the change for future use and updates local state tracking fields.
	*/
	void RecordStateChanges ()
	{
		// Compute change since last call.
		// lerpMove and lerpRotate are used to offset the movement incurred by lerping.
		Vector2 movementDelta = rb.position - positionAtPreviousFrame - lerpMove;
		float rotationDelta = rb.rotation - rotationAtPreviuosFrame - lerpRotate;
		if (movementDelta != Vector2.zero || rotationDelta != 0)
		{
			previousInputs.AddLast (new InputState (PhotonNetwork.time, movementDelta, rotationDelta));
			positionAtPreviousFrame = rb.position;
			rotationAtPreviuosFrame = rb.rotation;
		}
	}


	/**
	* Method that is invoked automatically every time the server sends a state update.
	*/
	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (!stream.isWriting)
		{
			// NOTE: order of calls in each section is significant, as is the order of the sections themselves
			// the order of these calls must correspond to the order of their counterpart calls on the server!!
			stream.Serialize (ref updatePosition);
			stream.Serialize (ref updateVelocity);
			stream.Serialize (ref updateRotation);

			// reset/update synch fields
			currentSynchDuration = 0;
			double updateTS = info.timestamp;
			UpdateSynchDuration (updateTS);

			previousUpdateTS = updateTS;
			UpdateServerPosition ();

			moveFrom = rb.position;
			rotateFrom = rb.rotation;

			lerpMove = Vector2.zero;
			lerpRotate = 0;
		}
		else
		{
			Debug.LogError ("Network player is attempting to update server");
		}
	}

	/**
	* Sets the total duration of the current synch as a function of the latest update timestamp, updateTSDeltaWieght and synchTimePadding
	* If client prediction is used, the total duration should be longer, as we don't want it to interfere with the client's inputs
	* and cause the client's model to jitter and teleport around.
	* If client prediction is disabled, we want the total duration to be slightly longer than the time between server updates,
	* in order to make the movement transitions smoother.
	*
	* Thus, we need to use different sets of variables for the computation, depending on whether client-side prediction is enabled.
	*/
	void UpdateSynchDuration (double newTS)
	{
		if (useClientPrediction)
		{
			predictionUpdateTSDeltaWeight = Mathf.Clamp (predictionUpdateTSDeltaWeight, 0, 1);
			totalSynchDuration = predictionUpdateTSDeltaWeight * totalSynchDuration + (1 - predictionUpdateTSDeltaWeight) * (newTS - previousUpdateTS) + prdictionSynchTimePadding;
		}
		else
		{
			updateTSDeltaWeight = Mathf.Clamp (updateTSDeltaWeight, 0, 1);
			totalSynchDuration = updateTSDeltaWeight * totalSynchDuration + (1 - updateTSDeltaWeight) * (newTS - previousUpdateTS) + synchTimePadding;
		}
	}

	/*
	* If client-side prediction is enabled, applies recorded state updates to the reported server position in order to bring
	* it up-to-date with the state of the client model in space.
	* If client-side prediciton is disabled, simply sets the body double's state to the state given by the server.
	*/
	void UpdateServerPosition ()
	{
		if (useClientPrediction)
		{
			// discard state updates older than the server update
			while (previousInputs.Count > 0 && previousInputs.First.Value.Timestamp < previousUpdateTS)
			{
				previousInputs.RemoveFirst ();
			}
			// apply state updates to server update in order to bring it up-to-date with client's inputs.
			foreach (InputState input in previousInputs)
			{
				updatePosition += input.MovementDelta;
				updateRotation += input.RotationDelta;
			}
		}
		// update the position of the rigidbody double
		// disable physics on bodyDouble in order to prevent its simulation from freaking out
		// while we change its positional info
		bodyDouble.Sleep ();
		bodyDouble.position = updatePosition;
		bodyDouble.rotation = updateRotation;
		// enable physics on bodyDouble when we are done
		bodyDouble.WakeUp ();
	}

	/**
	* Automatically destroys itself on disconnection.
	*/
	void OnLeftRoom ()
	{
		Destroy (gameObject);
	}

	/**
	* Called after destroy. Destroys the body double.
	*/
	void OnDestroy ()
	{
		Destroy (bodyDouble.gameObject);
	}
}
