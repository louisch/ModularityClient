using UnityEngine;
using System.Collections;

// updater for object not owned by player
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
public class NetworkPlayerUpdater : MonoBehaviour, IUpdater {
	public PhotonPlayer Owner {get; set;}
	public PhotonView View {get; private set;}
	public Rigidbody BodyDouble {get; set;}
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

	public float alpha = 0.8f; // weight attributed to previous update delta
	public float paddingTime = 0.008f; // padding time in ms added to the synch duration at each update 

	Vector3 updatePosition;
	Vector3 updateVelocity;
	Vector3 currentPosition;

	double previousUpdateTS; // timestamp indicating when the last server update has been received
	double currentSynchDuration; // time the last synch has been happening for
	double totalSynchDuration; // average time between successive updates

	Rigidbody rb;

	// initialise time of previous update TS to time of initialisation
	void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		View = GetComponent<PhotonView> ();
		previousUpdateTS = Time.time;
		totalSynchDuration = PhotonNetwork.sendRateOnSerialize + paddingTime;
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

	// Updates the total lerp time for each update. Alpha is used in order to soften updates that are abnormally far apart.
	void UpdateSynchDuration (double newTS, double previousTS)
	{
		alpha = Mathf.Clamp (alpha, 0, 1);
		totalSynchDuration = alpha * totalSynchDuration + (1 - alpha) * (newTS - previousTS) + paddingTime;
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
		Destroy (gameObject);
	}
}
