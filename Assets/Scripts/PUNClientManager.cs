using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PUNClientManager : MonoBehaviour {
	public float mod = 0.1f;
	float btnX, btnY, btnW, btnH;

	// cloud server connection setup
	public string gameVersion = "v0.1dev";
	bool connected = false;

	RoomInfo[] rooms;
	bool refreshing = false;

	// client net controller netview
	NetworkView nv;

	public GameObject playerModel;
	List<Player> players = new List<Player> ();

	// set up connection buttons (provisional)
	void Start ()
	{
		btnX = Screen.width * mod;
		btnY = Screen.height * mod;
		btnH = Screen.width * mod;
		btnW = Screen.width * mod;
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
			Debug.Log ("Got room " + room.name);
		}
	}

	void OnJoinedRoom ()
	{
		Debug.Log ("Joined room " + PhotonNetwork.room.name);
	}

	void OnGUI ()
	{
        if(!connected && GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Connect to server"))
		{
			Debug.Log ("Connecting to server");
			connected = ConnectToServer ();
		}
		else if (PhotonNetwork.room == null && rooms != null)
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
