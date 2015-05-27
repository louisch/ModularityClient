using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class Player : MonoBehaviour {
	// the player assigned by server to this object
	private NetworkPlayer player;
	private NetworkView nv;

	// save old data to only send updates
	private float prevh;
	private float prevv;

	void Awake ()
	{
		nv = GetComponent<NetworkView> ();
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
		Debug.Log ("setting player");
		this.player = player;

		if (player != Network.player)
		{ // else disable components related to it
			Debug.Log ("Disabling this player that is not ours");
			if (GetComponent<Transform> ().GetChild(0).gameObject)
			{
				Debug.Log ("Got child of player.");
				GetComponent<Transform> ().GetChild(0).gameObject.SetActive (false);
			}
		}
	}

	public NetworkPlayer GetPlayer ()
	{
		return player;
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
	void UpdateClientMotion (float h, float v)
	{
		// do nothing
	}
}
