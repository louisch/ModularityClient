using UnityEngine;
using System.Collections;


/**
* IUpdater implementation for objects that are not controlled directly by the local player.
*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PhotonView))]
public class NetworkPlayerUpdater : MonoBehaviour, IUpdater {
	/* Accessors! */
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

	/* Synchronisation variables used to create smooth transition between server position updates. */
	double previousUpdateTS; // timestamp indicating when the last server update has been received
	double currentSynchDuration; // time the last synch has been happening for
	double totalSynchDuration; // average time between successive updates
	// variables used to compute totalSynchDuration
	public float updateTSDeltaWeight = 0.1f; // weight attributed to previous update delta
	public float paddingTime = 0.15f; // padding time in ms added to the synch duration at each update 

	/* Target position reported by the server. */
	Vector2 updatePosition;
	Vector2 updateVelocity;
	Vector2 currentPosition;

	/* Reference to object's rigid body. */
	Rigidbody2D rb;

	/* Simple Awake setup. */
	void Awake ()
	{
		rb = GetComponent<Rigidbody2D> ();
		View = GetComponent<PhotonView> ();
		// We use Time.time for this class, as absolute timing is inessential here.
		previousUpdateTS = Time.time;
		totalSynchDuration = updateTSDeltaWeight;
	}

	/* Sets up ownership information. */
	public void SetupSpawn (PhotonPlayer owner, int viewID)
	{
		Owner = owner;
		ViewID = viewID;
	}

	/* Invoked upon server update. Resets/updates state synch fields. */
	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Debug.LogWarning ("Network player is attempting write");
		}
		else
		{
			// NOTE: do not change call order of anything in here
			stream.Serialize (ref updatePosition);
			stream.Serialize (ref updateVelocity);
			currentPosition = rb.position;

			currentSynchDuration = 0;
			double currentTime = Time.time;
			UpdateSynchDuration (currentTime);
			previousUpdateTS = currentTime;

			updatePosition += updateVelocity * (float)totalSynchDuration;
		}
	}

	/**
	* Computes totalSynchDuration for given timestamp. See LocalPlayerUpdater implementation for more information on synch duration.
	*/
	void UpdateSynchDuration (double newTS)
	{
		updateTSDeltaWeight = Mathf.Clamp (updateTSDeltaWeight, 0, 1);
		totalSynchDuration = updateTSDeltaWeight * totalSynchDuration + (1 - updateTSDeltaWeight) * (newTS - previousUpdateTS) + paddingTime;
	}

	/**
	* Called at update instead of FixedUpdate as this does not involve any physics.
	*/
	void FixedUpdate ()
	{
		LerpToUpdate ();
	}

	/**
	* Smoothly transitions between previous server state and latest server state.
	*/
	void LerpToUpdate ()
	{
		// we use smoothDeltaTime, as it is averaged across all deltaTimes, giving smoother movement.
		currentSynchDuration += Time.deltaTime;
		rb.position = Vector2.Lerp (currentPosition, updatePosition, (float)(currentSynchDuration/totalSynchDuration));
	}

	/**
	* Self-destructs on disconnection.
	*/
	void OnLeftRoom ()
	{
		Destroy (gameObject);
	}
}
