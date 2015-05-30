using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
*	This class applies to any game object shared over the network.
*/
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(ObjectUpdater))]
public class PunNetworkObject : MonoBehaviour {

	// client-side prediction vars
	public bool serverCorrectionEnabled = true;
	public float positionErrorThreshold = 0.2f;
	double serverTS;      // timestamp of last canonical state
	Vector3 serverPos;    // position of last canonical state
	Quaternion serverRot; // rotation of last canonical state

	// object components
	public ObjectUpdater objectUpdater;
	Transform trans;
	Rigidbody rb;

	void Awake ()
	{
		serverTS = PhotonNetwork.time; // creation time
		trans = GetComponent<Transform> ();
	}

	// on update from server, gets updated transform details, polls updater and 
	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		Debug.Log ("Serialising");
		if (stream.isWriting)
		{
			Debug.LogError ("Client is attempting to write");
		}
		else if (serverCorrectionEnabled)
		{
			Debug.Log ("Got server update");
			double updateTS = info.timestamp;
			if (updateTS < serverTS)
			{
				// discard update if its timestamp is less than the timestamp of the last update
				Debug.LogError ("Server is going back in time!!");
				return;
			}
			// retrieve updates from server
			stream.Serialize (ref serverPos);
			stream.Serialize (ref serverRot);

			serverTS = updateTS;

			Debug.Log ("Polling object updater");
			objectUpdater.UpdatePos (ref serverPos, updateTS);
			StartCoroutine(ApplyCorrection (objectUpdater.LerpTime));
		}
	}

	// lerps object to canonical positions given by server over time
	IEnumerator ApplyCorrection (double duration)
	{
		if (duration == 0)
		{
			Debug.LogError ("Lerp time for object is 0");
			yield break;
		}
		float distance = Vector3.Distance (trans.position, serverPos);
		double startTime = PhotonNetwork.time;
		double elapsed = 0;

		Debug.LogFormat ("Moving from {0} towards {1}", trans.position, serverPos);

		// correct for duration while beyond the error margin
		while (distance >= positionErrorThreshold && elapsed < duration)
		{
			trans.position = Vector3.Lerp (trans.position, serverPos, (float) (elapsed/duration));
			// trans.rotation = Quaternion.Slerp (trans.rotation, serverRot, lerp);
			distance = Vector3.Distance (trans.position, serverPos);
			elapsed = PhotonNetwork.time - startTime;
			yield return null;
		}
	}

	void OnDisconnectedFromPhoton ()
	{
		Destroy (gameObject);
	}
}

