using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
	// the player assigned by server to this object
	NetworkPlayer player;
	NetworkView nv;

	// save old data to only send updates
	float prevh;
	float prevv;

	// client-side prediction vars
	Vector3 serverPos;    // position of last canonical state
	Quaternion serverRot; // rotation of last canonical state
	double serverTS;      // timestamp of last canonical state
	LinkedList<InputState> previousInputs = new LinkedList<InputState> ();

	public float hSpeed = 10;
	public float vSpeed = 10;
	public double lerpSpeed = 4;
	public bool serverCorrectionEnabled = true;
	public float positionErrorThreshold = 0.1f;

	// object components
	Transform trans;
	Rigidbody rb;

	void Awake ()
	{
		nv = GetComponent<NetworkView> ();

		rb = GetComponent<Rigidbody> ();
		trans = GetComponent<Transform> ();
		serverTS = Network.time;
	}

	// Setters-getters for view ID
	public void SetViewID (NetworkViewID viewID)
	{
		nv.viewID = viewID;
	}

	public NetworkViewID GetViewID ()
	{
		return nv.viewID;
	}

	// Setters-getters for NetworkPlayer of this object
	public void SetPlayer (NetworkPlayer player)
	{
		Debug.Log ("Setting object ownership");
		this.player = player;

		if (player != Network.player)
		{ // else disable components related to it
			Debug.Log ("Disabling camera of player as it is not ours");
			if (GetComponent<Transform> ().GetChild(0).gameObject)
			{
				Debug.Log ("Camera found and disabled");
				GetComponent<Transform> ().GetChild(0).gameObject.SetActive (false);
			}
		}
	}

	public NetworkPlayer GetPlayer ()
	{
		return player;
	}

	// movement update for this object
	void FixedUpdate ()
	{
		if (Network.isServer)
		{ // do not update if we are somehow server
			Debug.LogError ("Client launched in server mode!");
			return;
		}
		if (Network.player == player)
		{ // send updates to server
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			// only send an update if input has changed
			if (h != prevh || v != prevv)
			{
				nv.RPC ("UpdateInput", RPCMode.Server, h, v);
				prevh = h;
				prevv = v;
			}

			// normalise input vecor
			Vector3 moveBy = new Vector3 (h,0,v).normalized;
			// compute movement delta vector (frame independent)
			moveBy = new Vector3(moveBy.x * hSpeed * Time.fixedDeltaTime, 0, moveBy.z * vSpeed * Time.fixedDeltaTime);
			// save vector with current timestamp
			previousInputs.AddLast (new InputState (Network.time, moveBy));
			// apply movement vector to position
			rb.MovePosition(trans.position + moveBy);
		}
	}

	[RPC]
	void UpdateInput (float h, float v)
	{
		// do nothing
		return;
	}

	// on server update, replay inputs and apply correction (if applicable)
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		Debug.Log ("serialisation is taking place");	
		if (stream.isWriting)
		{
			Debug.LogError ("Client is attempting to write");
		}
		else
		{
			Debug.Log ("Getting server update");
			if (serverCorrectionEnabled)
			{
				double updateTS = info.timestamp; // save timestamp for efficiency reasons
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
				// if object is current player, apply previoius input to server position
				if (Network.player == player)
				{
					Debug.Log ("looking for input timestamp");
					// discard inputs that are too old
					while (previousInputs.Count > 0 && previousInputs.First.Value.timestamp < updateTS)
					{
						previousInputs.RemoveFirst ();
					}
					Debug.Log ("Applying past inputs to server update");
					// apply inputs to position received
					foreach (InputState input in previousInputs)
					{
						serverPos += input.moveBy;
					}
				}
				// correct our position to that of the server
				StartCoroutine(ApplyCorrection (lerpSpeed));
			}
		}
	}

	// lerps to position overTime (in msec)
	IEnumerator ApplyCorrection (double overTime)
	{
		float distance = Vector3.Distance (trans.position, serverPos);
		double elapsedTime = Network.time;
		double endTime = elapsedTime + overTime;

		// correct only if we are beyond the error margin
		while (distance >= positionErrorThreshold && elapsedTime < endTime)
		{
			trans.position = Vector3.Lerp (trans.position, serverPos, (float) (elapsedTime/endTime));
			distance = Vector3.Distance (trans.position, serverPos);
			// trans.rotation = Quaternion.Slerp (trans.rotation, serverRot, lerp);
			yield return null;
		}
	}

	void OnDisconnectedFromServer ()
	{
		Destroy (gameObject);
	}
}
