using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ClientManager : MonoBehaviour {

	public float mod = 0.1f;
	public string gameName = "Cooking_Foxes";
	public GameController controller;

	public static ClientManager instance;
	private NetworkView nv;

	public Transform playerModel;

	private float btnX, btnY, btnW, btnH;
	private HostData[] hosts;
	private bool refreshing = false;

	void Start ()
	{
		// singleton pattern for the network manager - there should only be one
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		nv = GetComponent<NetworkView> ();

		btnX = Screen.width * mod;
		btnY = Screen.height * mod;
		btnH = Screen.width * mod;
		btnW = Screen.width * mod;
	}
	public NetworkView GetNetView ()
	{
		return nv;
	}

	void fetchServerList ()
	{
		MasterServer.ClearHostList ();
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}

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

	[RPC]
	void SpawnPlayer (NetworkPlayer player, NetworkViewID viewID)
	{
		Debug.Log ("Got instruction to spawn player");
		Transform newplayer = Instantiate(playerModel) as Transform;
		newplayer.GetComponent<Player> ().SetPlayer (player);
		newplayer.GetComponent<NetworkView> ().viewID = viewID;
	}


	void OnGUI ()
	{
		if (!Network.isClient && !Network.isServer)
		{
	        if(GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Find Server"))
			{
				Debug.Log ("Finding Servers...");
				fetchServerList ();
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
