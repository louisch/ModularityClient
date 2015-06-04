using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
* Script used for establishing connection with server, selecting game room, etc.
* Additionally, controls player and object spawning.
*/
[RequireComponent(typeof(PhotonView))]
public class PunClientManager : MonoBehaviour {
	/* Accessorsssssss */
	PhotonView View {get; set;}

	/* Button setup. To be removed later. */
	public float mod = 0.1f;
	float btnX, btnY, btnW, btnH;

	/* Cloud server connection setup information. */
	public string gameVersion = "v0.1dev";
	bool connected = false;
	RoomInfo[] rooms;

	ObjectConstructor constructor;

	/* Hear my call and come forth! */
	void Awake ()
	{
		btnX = Screen.width * mod;
		btnY = Screen.height * mod;
		btnH = Screen.width * mod;
		btnW = Screen.width * mod;

		View = GetComponent<PhotonView> ();
		constructor = GetComponent<ObjectConstructor> ();
	}

	/* Called to connect to cloud server. */
	bool ConnectToCloudServer ()
	{
		Debug.Log ("Connecting to server");
		return PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	/* Automatically invoked when photon cloud sends room list updates. */
	void OnReceivedRoomListUpdate ()
	{
		Debug.Log ("Got room list");
		rooms = PhotonNetwork.GetRoomList ();
	}

	/**
	* Automatically called when room is joined.
	* Asks server to spawn this player.
	*/
	void OnJoinedRoom ()
	{
		Debug.Log ("Joined room " + PhotonNetwork.room.name);
		View.RPC ("RequestSpawn", PhotonTargets.MasterClient);
	}

	/**
	* Invoked by room server when a player needs to be spawned in.
	*/
	[RPC]
	void SpawnPlayer (PhotonPlayer player, int statusID, int controllerID, Vector2 pos, float rot)
	{
		Debug.LogFormat ("Spawning player {0} with id {1}, {2}", player.ToString(), statusID, controllerID);
		constructor.ConstructPlayer (player, statusID, controllerID, pos, rot);
	}

	/**
	* Called automatically when disconnected from photon cloud
	*/
	void OnDisconnectedFromPhoton ()
	{
		Debug.Log ("Disconnected from cloud");
		connected = false;
	}

	/**
	* Called automatically when a player disconnects from the game. Despawns the player in question.
	*/
	void OnPhotonPlayerDisconnected (PhotonPlayer disconnected)
	{
		Debug.Log ("Player " + disconnected.ToString () + " disonnectedPlayer.");
	}

	/* Provisional button setup. */
	void OnGUI ()
	{
        if(!connected && GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Connect to server"))
		{
			Debug.Log ("Connecting to server");
			connected = ConnectToCloudServer ();
		}
		else if (connected && PhotonNetwork.room == null && rooms != null)
		{
			for (int i = 0; i < rooms.Length; ++i)
			{
				if (GUI.Button (new Rect (btnX * 2 + btnW, btnY + 1 + (btnH*i), btnW*3, btnW), "Join " + rooms[i].name))
				{
					Debug.Log ("Connecting to room");
					PhotonNetwork.JoinRoom (rooms[i].name);
				}
			}
		}
	}
}
