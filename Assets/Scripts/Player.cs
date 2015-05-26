using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Player : MonoBehaviour {
	// the player assigned by server to this object
	private NetworkPlayer player;

	// save old data to only send updates
	private float prevh;
	private float prevv;

	public void SetViewID (NetworkViewID viewID)
	{
		GetComponent<NetworkView> ().viewID = viewID;
	}

	[RPC]
	public void SetPlayer (NetworkPlayer player)
	{
		Debug.Log ("setting player");
		this.player = player;

		if (player == Network.player)
		{ // if we are this player, enable it
			enabled = true;
		}
		else
		{ // else diable components related to it
			if (GetComponent<Transform> ().GetChild(0).gameObject)
			{
				GetComponent<Transform> ().GetChild(0).gameObject.SetActive (false);
			}
			if (GetComponent<AudioListener> ())
			{
				GetComponent<AudioListener> ().enabled = false;
			}
		}
	}

	[RPC]
	NetworkPlayer GetPlayer ()
	{
		return player;
	}

	void Awake ()
	{
		if (Network.isClient)
		{
			enabled = false;
		}
	}

	void Update ()
	{
		if (Network.isServer)
		{ // do not update if we are somehow server
			Debug.Log ("How am *I* a server?!?");
			return;
		}
		if (player != null && Network.player == player)
		{ // send updates to server
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			if (h != prevh || v != prevv)
			{
				GetComponent<NetworkView> ().RPC ("UpdateClientMotion",
													RPCMode.Server,
													h, v);
				prevh = h;
				prevv = v;
			}
		}
	}

	[RPC]
	void UpdateClientMotion (float v, float h)
	{
		//
	}
	
}
