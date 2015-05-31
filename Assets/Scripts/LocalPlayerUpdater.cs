using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class LocalPlayerUpdater : MonoBehaviour, IUpdater {
	public PhotonPlayer Owner {get; set;}
	public PhotonView View {get; private set;}
	public int ViewID
	{
		get
		{
			return View.viewID;
		}
		set
		{
			View.viewID = value;
		}
	}

	public GameObject GameObject
	{
		get
		{
			return gameObject;
		}
	}

	public Rigidbody BodyDouble {get; set;}

	public double alpha = 0.5; // weight attributed to previous update delta

	// info saved before sending network updates
	float pHInput;
	float pVInput;

	// movement related refs
	Transform trans;
	Rigidbody rb;

	// movement vars
	public float hSpeed = 10;
	public float vSpeed = 10;

	// state reconciliation vars
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();

	Vector3 updatePosition;
	Vector3 updateVelocity;
	Vector3 positionOnPreviousFrame;

	double previousUpdateTS;
	double currentSynchDuration = 0;
	double totalSynchDuration = 100; // assume 100ms between updates to begin with

	void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		trans = GetComponent<Transform> ();
		View = GetComponent<PhotonView> ();

		previousUpdateTS = PhotonNetwork.time;
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
			double updateTS = info.timestamp;
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
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// only send an update if input has changed
		if (h != pHInput || v != pVInput)
		{
			Debug.Log ("Sending position update to server");
			View.RPC ("UpdateInput", PhotonTargets.MasterClient, h, v);
			pHInput = h;
			pVInput = v;
		}
		currentSynchDuration += Time.fixedDeltaTime;

		// move code
		Vector3 moveBy = new Vector3 (h,0,v).normalized;
		moveBy = new Vector3(moveBy.x * hSpeed * Time.fixedDeltaTime, 0, moveBy.z * vSpeed * Time.fixedDeltaTime);
		updatePosition += moveBy;

		rb.position = Vector3.Lerp (trans.position + moveBy, updatePosition, (float)(currentSynchDuration/totalSynchDuration));

		Vector3 movedBy = rb.position - positionOnPreviousFrame;
		if (movedBy != Vector3.zero)
		{
			previousInputs.AddLast (new InputState (PhotonNetwork.time, movedBy));
			positionOnPreviousFrame = rb.position;
		}
	}


	// Updates the total lerp time for each update. Alpha is used in order to soften updates that are abnormally far apart.
	void UpdateSynchDuration (double newTS)
	{
		totalSynchDuration = alpha * totalSynchDuration + (1 - alpha) * (newTS - previousUpdateTS);
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
		while (previousInputs.Count > 0 && previousInputs.First.Value.timestamp < previousUpdateTS)
		{
			previousInputs.RemoveFirst ();
		}
		Debug.Log ("Applying past inputs to server update");
		foreach (InputState input in previousInputs)
		{
			updatePosition += input.moveBy;
		}
		// update the position of the rigid body double
		BodyDouble.position = updatePosition;
	}

	void OnDisconnectedFromPhoton ()
	{
		Destroy (BodyDouble.gameObject);
		Destroy (gameObject);
	}
}
