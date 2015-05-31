﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class PunClientManager : MonoBehaviour {
	public float mod = 0.1f;
	float btnX, btnY, btnW, btnH;

	// cloud server connection setup
	public string gameVersion = "v0.1dev";
	bool connected = false;

	RoomInfo[] rooms;

	// client net controller netview
	PhotonView View {get; set;}

	public GameObject playerModel;
	public GameObject playerBodyDouble;
	public GameObject networkPlayerModel;
	List<IUpdater> players = new List<IUpdater> ();

	// set up connection buttons (provisional)
	void Start ()
	{
		btnX = Screen.width * mod;
		btnY = Screen.height * mod;
		btnH = Screen.width * mod;
		btnW = Screen.width * mod;

		View = GetComponent<PhotonView> ();
	}

	bool ConnectToServer ()
	{
		Debug.Log ("Connecting to server");
		return PhotonNetwork.ConnectUsingSettings(gameVersion);
	}

	// update room list on update
	void OnReceivedRoomListUpdate ()
	{
		Debug.Log ("Retreiving room list");
		rooms = PhotonNetwork.GetRoomList ();
		foreach (RoomInfo room in rooms)
		{
			Debug.Log ("Got room: " + room.name);
		}
	}

	void OnJoinedRoom ()
	{
		Debug.Log ("Joined room " + PhotonNetwork.room.name);
		View.RPC ("RequestSpawn", PhotonTargets.MasterClient);
	}

	void OnDisconnectedFromPhoton ()
	{
		Debug.Log ("Disconnected from server");
		connected = false;
	}

	[RPC]
	void SpawnPlayer (PhotonPlayer player, int viewID)
	{
		Debug.LogFormat ("Spawning player {0} with id {1}", player.ToString(), viewID);
		GameObject handle;
		Rigidbody bodyDouble = null;
		if (player.isLocal)
		{
			handle = Instantiate(playerModel) as GameObject;
			bodyDouble = Instantiate(playerBodyDouble).GetComponent<Rigidbody> ();
		}
		else
		{
			handle = Instantiate(networkPlayerModel) as GameObject;
		}
		IUpdater updater = handle.GetComponent<IUpdater> ();
		if (updater == null)
		{
			Debug.LogError ("Player does not have an updater");
		}
		updater.BodyDouble = bodyDouble;
		updater.Owner = player;
		updater.ViewID = viewID;
		players.Add (updater);
		
	}

	[RPC]
	void DespawnPlayer (PhotonPlayer player)
	{
		foreach (IUpdater p in players)
		{
			if (p.Owner == player)
			{
				Destroy (p.GameObject);
				players.Remove (p);
			}
		}
	}

	void OnGUI ()
	{
        if(!connected && GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Connect to server"))
		{
			Debug.Log ("Connecting to server");
			connected = ConnectToServer ();
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
