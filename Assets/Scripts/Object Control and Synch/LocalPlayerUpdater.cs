using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
public class LocalPlayerUpdater : MonoBehaviour, IUpdater {
	// general access fields
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

	// fields used for server reconciliation
	public GameObject playerBodyDouble;
	Rigidbody2D bodyDouble; // used to extrapolate current server position from server updates
	Vector2 positionAtPreviousFrame; // used to record by how much player moves between calls to FixedUpdate
	float rotationAtPreviuosFrame;
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();

	Vector2 moveFrom;
	float rotateFrom;
	Vector2 lerpMove;
	float lerpRotate;
	Vector2 updatePosition; // position reported by server
	Vector2 updateVelocity; // velocity reported by server - not necessary for players
	float updateRotation;

	double previousUpdateTS; // timestamp of previous server update
	double currentSynchDuration = 0; // time the current synch has been running for
	double totalSynchDuration; // time over which positions are synched
	public float updateTSDeltaWeight = 0.2f; // weight attributed to previous update delta
	public float synchTimePadding = 0.1f; // pads total synch duration for smoother movement
	public double inputTSPadding = 0.1f;

	public bool useClientPrediction = true;

	// fields used to determine whether input changed
	float previousStrafe;
	float previousThrust;
	float previousTorque;

	// movement code related fields
	public float strafeModifier = 100;
	public float thrustModifier = 100;
	public float torqueModifier = 5;

	Rigidbody2D rb;

	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		View = GetComponent<PhotonView> ();

		positionAtPreviousFrame = rb.position;
		rotationAtPreviuosFrame = rb.rotation;

		previousUpdateTS = PhotonNetwork.time;
		totalSynchDuration = 1 / PhotonNetwork.sendRateOnSerialize;
	}


	public void SetupSpawn (PhotonPlayer owner, int viewID)
	{
		Owner = owner;
		ViewID = viewID;
		bodyDouble = (Instantiate(playerBodyDouble, rb.position, Quaternion.identity) as GameObject).GetComponent<Rigidbody2D> ();
		bodyDouble.rotation = rb.rotation;
	}


	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Debug.LogWarning ("Network player is attempting write");
		}
		else
		{
			Debug.Log ("Receiving server corrections");
			stream.Serialize (ref updatePosition);
			stream.Serialize (ref updateVelocity);
			stream.Serialize (ref updateRotation);

			currentSynchDuration = 0;
			double updateTS = info.timestamp;
			UpdateSynchDuration (updateTS);

			previousUpdateTS = updateTS;
			UpdateServerPosition ();

			moveFrom = rb.position;
			rotateFrom = rb.rotation;

			lerpMove = Vector2.zero;
		}
	}

		// movement update for this object
	void FixedUpdate ()
	{
		if (!Owner.isLocal)
		{
			Debug.LogError ("Player script attached to non-player object!");
			return;
		}
		// get new inputs
		float thrust = InputManager.Instance.ThrustAxis;
		float strafe = InputManager.Instance.StrafeAxis;
		float torque = InputManager.Instance.TorqueAxis;

		// check if inputs changed since last call to FixedUpdate and send update to server if true
		if (strafe != previousStrafe || thrust != previousThrust || previousTorque != torque)
		{
			Debug.Log ("Sending position update to server");
			View.RPC ("UpdateInput", PhotonTargets.MasterClient, strafe, thrust, torque);
			previousStrafe = strafe;
			previousThrust = thrust;
			previousTorque = torque;
		}

		currentSynchDuration += Time.fixedDeltaTime;

		if (useClientPrediction)
		{
			CheckChanges ();
			moveFrom = rb.position;
			rotateFrom = rb.rotation;

			// get movement forces
			Vector2 moveForce = new Vector2 (strafe,thrust).normalized;
			moveForce = new Vector2(moveForce.x * strafeModifier * Time.fixedDeltaTime, moveForce.y * thrustModifier * Time.fixedDeltaTime);
			float torqueValue = torque * torqueModifier * Time.fixedDeltaTime;

			// apply forces to client model
			rb.AddForce (moveForce);
			rb.AddTorque (torqueValue);
			// apply forces to server model
			bodyDouble.AddForce (moveForce);
			bodyDouble.AddTorque (torqueValue);
		}

		// slowly synch between server position and player position
		rb.position = Vector2.Lerp (moveFrom, bodyDouble.position, (float)(currentSynchDuration/totalSynchDuration));
		lerpMove = rb.position - moveFrom;
		rb.rotation = Mathf.Lerp (rotateFrom, bodyDouble.rotation, (float)(currentSynchDuration/totalSynchDuration));
		lerpRotate = rb.rotation - rotateFrom;
	}

	void CheckChanges ()
	{
		// record movement since last FixedUpdate
		Vector2 movementDelta = rb.position - positionAtPreviousFrame - lerpMove;
		float rotationDelta = rb.rotation - rotationAtPreviuosFrame - lerpRotate;
		if (movementDelta != Vector2.zero || rotationDelta != 0)
		{
			previousInputs.AddLast (new InputState (PhotonNetwork.time, movementDelta, rotationDelta));
			positionAtPreviousFrame = rb.position;
			rotationAtPreviuosFrame = rb.rotation;
		}
	}


	// Updates the total lerp time for each update. updateTSDeltaWeight is used in order to soften updates that are abnormally far apart.
	void UpdateSynchDuration (double newTS)
	{
		updateTSDeltaWeight = Mathf.Clamp (updateTSDeltaWeight, 0, 1);
		totalSynchDuration = updateTSDeltaWeight * totalSynchDuration + (1 - updateTSDeltaWeight) * (newTS - previousUpdateTS) + synchTimePadding;
	}

	// applies inputs in order to bring reported serverPos up to current time
	void UpdateServerPosition ()
	{
		if (!Owner.isLocal)
		{
			Debug.LogError ("Player script attached to non-player object!");
			return;
		}
		Debug.Log ("Updating reported server position");
		while (previousInputs.Count > 0 && previousInputs.First.Value.Timestamp + inputTSPadding < previousUpdateTS)
		{
			previousInputs.RemoveFirst ();
		}
		foreach (InputState input in previousInputs)
		{
			updatePosition += input.MovementDelta;
			updateRotation += input.RotationDelta;
		}
		// update the position of the rigid body double
		// disable physics on bodyDouble in order to prevent its simulation from freaking out when we change its positional info
		bodyDouble.Sleep ();
		bodyDouble.position = updatePosition;
		bodyDouble.rotation = updateRotation;
		// enable physics on bodyDouble when we are done
		bodyDouble.WakeUp ();
	}

	void OnDisconnectedFromPhoton ()
	{
		Despawn ();
	}

	public void Despawn ()
	{
		Destroy (bodyDouble.gameObject);
		Destroy (gameObject);
	}
}
