using UnityEngine;
using System.Collections;

// updater for object not owned by player
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class NetworkPlayerUpdater : MonoBehaviour, IUpdater {
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

	public float updateTSDeltaWeight = 0.1f; // weight attributed to previous update delta
	public float paddingTime = 0.15f; // padding time in ms added to the synch duration at each update 

	Vector3 updatePosition;
	Vector3 updateVelocity;
	Vector3 currentPosition;

	double previousUpdateTS; // timestamp indicating when the last server update has been received
	double currentSynchDuration; // time the last synch has been happening for
	public double totalSynchDuration; // average time between successive updates

	Rigidbody rb;

	// initialise time of previous update TS to time of initialisation
	void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		View = GetComponent<PhotonView> ();
		previousUpdateTS = Time.time;
		totalSynchDuration = 1 / PhotonNetwork.sendRateOnSerialize;
	}

	public void SetupSpawn (PhotonPlayer owner, int viewID)
	{
		Owner = owner;
		ViewID = viewID;
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Debug.LogWarning ("Network player is attempting write");
		}
		else
		{
			stream.Serialize (ref updatePosition);
			stream.Serialize (ref updateVelocity);
			currentPosition = rb.position;

			currentSynchDuration = 0;
			double currentTime = Time.time;
			UpdateSynchDuration (currentTime, previousUpdateTS);
			previousUpdateTS = currentTime;

			updatePosition += updateVelocity * (float)totalSynchDuration;
		}
	}

	// Updates the total lerp time for each update. updateTSDeltaWeight is used in order to soften updates that are abnormally far apart.
	void UpdateSynchDuration (double newTS, double previousTS)
	{
		updateTSDeltaWeight = Mathf.Clamp (updateTSDeltaWeight, 0, 1);
		totalSynchDuration = updateTSDeltaWeight * totalSynchDuration + (1 - updateTSDeltaWeight) * (newTS - previousTS) + paddingTime;
	}

	void Update ()
	{
		LerpToUpdate ();
	}

	// transitions from current position to the position given by the server
	void LerpToUpdate ()
	{
		currentSynchDuration += Time.smoothDeltaTime;
		rb.position = Vector3.Lerp (currentPosition, updatePosition, (float)(currentSynchDuration/totalSynchDuration));
	}

	void OnDisconnectedFromPhoton ()
	{
		Despawn ();	
	}

	public void Despawn ()
	{
		Destroy (gameObject);
	}
}
