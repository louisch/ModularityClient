using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ClientManager : MonoBehaviour {

	// button setup
	public float mod = 0.1f;
	private float btnX, btnY, btnW, btnH;

	// master server connection setup
	public string gameName = "Cooking_Foxes";
	private HostData[] hosts;
	private bool refreshing = false;

	// client net controller netview
	private NetworkView nv;

	public GameObject playerModel;
	private List<Player> players = new List<Player> ();

	// set up connection buttons (provisional)
	void Start ()
	{
		nv = GetComponent<NetworkView> ();

		btnX = Screen.width * mod;
		btnY = Screen.height * mod;
		btnH = Screen.width * mod;
		btnW = Screen.width * mod;
	}

	// get server list from master server
	void FetchServerList ()
	{
		MasterServer.ClearHostList ();
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}

	// display server list when not in a game
	void Update ()
	{
		hosts = MasterServer.PollHostList ();
		if (refreshing && hosts.Length > 0)
		{
			refreshing = false;
			foreach (HostData host in hosts)
			{
				Debug.Log ("Game Name: " + host.gameName);
			}
			
		}
	}

	void OnConnectedToServer ()
	{
		Debug.Log ("Connected to server. Disabling message queue!");
		// Network.isMessageQueueRunning = false;
		// Level Loading code goes here
		// void OnLevelWasLoaded -> re-enable message queue
		nv.RPC ("RequestSpawn", RPCMode.Server, Network.player);
	}


	[RPC]
	void RequestSpawn (NetworkPlayer requestee)
	{
		if (requestee != Network.player)
		{
			Debug.LogError ("Spawn request in client space!");
		}
		else Debug.Log ("Requested Spawn.");
	}

	[RPC]
	void SpawnPlayer (NetworkPlayer player, NetworkViewID viewID)
	{
		Debug.Log ("Got instruction to spawn player " + player.ToString());
		GameObject handle = Instantiate(playerModel) as GameObject;
		Player man = handle.GetComponent<Player> ();
		if (man == null)
		{
			Debug.LogError ("This player has no manager!");
		}
		man.SetPlayer (player);
		man.SetViewID (viewID);
		players.Add (man);
	}

	[RPC]
	void DespawnPlayer (NetworkPlayer player)
	{
		foreach (Player p in players)
		{
			if (p.GetPlayer () == player)
			{
				Destroy (p.gameObject);
				players.Remove (p);
			}
		}
	}


	void OnGUI ()
	{
		if (!Network.isClient && !Network.isServer)
		{
	        if(GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Find Server"))
			{
				Debug.Log ("Finding Servers...");
				FetchServerList ();
			}
			for (int i = 0; i < hosts.Length; ++i)
			{
				if (GUI.Button (new Rect (btnX * 2 + btnW, btnY + 1 + (btnH*i), btnW*3, btnW), hosts[i].gameName))
				{
					Network.Connect (hosts[i], "foxesinboxes");
				}
			}
		}
	}
}
