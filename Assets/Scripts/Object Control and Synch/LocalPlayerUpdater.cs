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
	Vector2 positionOnPreviousFrame; // used to record by how much player moves between calls to FixedUpdate
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();

	Vector2 updatePosition; // position reported by server
	Vector2 updateVelocity; // velocity reported by server - not necessary for players

	double previousUpdateTS; // timestamp of previous server update
	double currentSynchDuration = 0; // time the current synch has been running for
	public double totalSynchDuration; // time over which positions are synched
	public float updateTSDeltaWeight = 0.9f; // weight attributed to previous update delta
	public float synchTimePadding = 0.2f; // pads total synch duration for smoother movement
	public double inputTSPadding = 0.1f;

	public bool useClientPrediction = true;

	// fields used to determine whether input changed
	float previousStrafeAxis;
	float previousThrustAxis;

	// movement code related fields
	public float strafeSpeed = 10;
	public float thrustSpeed = 10;

	Rigidbody2D rb;

	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		View = GetComponent<PhotonView> ();

		previousUpdateTS = Time.time;
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

			currentSynchDuration = 0;
			double updateTS = Time.time;
			UpdateSynchDuration (updateTS);

			previousUpdateTS = updateTS;
			UpdateServerPosition ();

			updatePosition += updateVelocity * (float)totalSynchDuration;
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
		// send updates to server
		float thrust = InputManager.Instance.ThrustAxis;
		float strafe = InputManager.Instance.StrafeAxis;

		// only send an update if input has changed
		if (strafe != previousStrafeAxis || thrust != previousThrustAxis)
		{
			Debug.Log ("Sending position update to server");
			View.RPC ("UpdateInput", PhotonTargets.MasterClient, strafe, thrust);
			previousStrafeAxis = strafe;
			previousThrustAxis = thrust;
		}

		Vector2 newPosition = rb.position;
		currentSynchDuration += Time.fixedDeltaTime;

		if (useClientPrediction)
		{
			// record movement since last FixedUpdate
			Vector2 movedBy = rb.position - positionOnPreviousFrame;
			if (movedBy != Vector2.zero)
			{
				previousInputs.AddLast (new InputState (Time.time, movedBy));
				positionOnPreviousFrame = rb.position;
			}

			// move client and update server position simulation
			Vector2 moveBy = new Vector2 (strafe,thrust).normalized;
			moveBy = new Vector2(moveBy.x * strafeSpeed * Time.fixedDeltaTime, moveBy.y * thrustSpeed * Time.fixedDeltaTime);
			newPosition += moveBy;
			// move server simulation
			bodyDouble.position += moveBy;
		}

		// slowly synch between server position and player position
		rb.position = Vector2.Lerp (newPosition, bodyDouble.position, (float)(currentSynchDuration/totalSynchDuration));
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
		Debug.Log ("looking for input timestamp");
		while (previousInputs.Count > 0 && previousInputs.First.Value.timestamp + inputTSPadding < previousUpdateTS)
		{
			previousInputs.RemoveFirst ();
		}
		Debug.Log ("Applying past inputs to server update");
		foreach (InputState input in previousInputs)
		{
			updatePosition += input.movedBy;
		}
		// update the position of the rigid body double
		bodyDouble.position = updatePosition;
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
