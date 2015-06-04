using UnityEngine;
using System.Collections;

public class ObjectConstructor : MonoBehaviour {
	// general variables
	float gravityScale = 0;

	// player variables
	public GameObject playerModule;
	public GameObject networkPlayerModule;

	public float defaultPlayerMass = 1;
	public float defaultPlayerDrag = 4;
	public float defaultPlayerAngularDrag = 10;
	public float playerSpawnZ = 3;

	public float cameraZ = 0;

	public GameObject ConstructPlayer (PhotonPlayer owner, int trackerID, int controllerID)
	{
		if (owner.isLocal)
		{
			return ConstructLocalPlayer (owner, trackerID, controllerID);
		}
		else
		{
			return ConstructNetworkPlayer (owner, trackerID, controllerID);
		}
	}

	/* Constructs a player object in game. */
	GameObject ConstructLocalPlayer (PhotonPlayer owner, int trackerID, int controllerID)
	{
		GameObject player = Instantiate (playerModule) as GameObject;
		player.name = "(construct)player " + owner.ToString ();
		player.transform.position = new Vector3 (0,0,playerSpawnZ);

		// add rigid body
		player.AddComponent<Rigidbody2D> ();
		Rigidbody2D rb = player.GetComponent<Rigidbody2D> ();
		rb.mass = defaultPlayerMass;
		rb.drag = defaultPlayerDrag;
		rb.angularDrag = defaultPlayerAngularDrag;
		rb.gravityScale = gravityScale;

		// add photon views
		player.AddComponent<PhotonView> ();
		player.AddComponent<PhotonView> ();
		PhotonView[] views = player.GetComponents<PhotonView> ();

		// setup status tracker
		player.AddComponent<ObjectStatusController> ();
		ObjectStatusController statusController = player.GetComponent<ObjectStatusController> ();
		statusController.view = views[0];
		statusController.StatusControllerID = trackerID;
		statusController.RB = rb;
		// setup view to use the tracker
		views[0].observed = statusController;
		views[0].synchronization = ViewSynchronization.UnreliableOnChange;

		// setup controller
		player.AddComponent<LocalPlayerController> ();
		LocalPlayerController controller = player.GetComponent<LocalPlayerController> ();
		controller.Owner = owner;
		controller.View = views[1];
		controller.ViewID = controllerID;
		controller.RB = rb;
		// setup view to observe the controller
		views[1].observed = controller;
		views[1].synchronization = ViewSynchronization.UnreliableOnChange;

		// setup body double for local player
		controller.bodydouble = SpawnBodyDouble (player);

		CreatePlayerCamera (player);

		return player;
	}

	/* Constructs a body double for the player. */
	Rigidbody2D SpawnBodyDouble (GameObject actor)
	{
		GameObject bodydouble = new GameObject ();
		bodydouble.name = "(construct)Bodydouble";
		bodydouble.transform.position = actor.transform.position;

		// add rigid body
		bodydouble.AddComponent<Rigidbody2D> ();
		Rigidbody2D rb = bodydouble.GetComponent<Rigidbody2D> ();
		rb.mass = defaultPlayerMass;
		rb.drag = defaultPlayerDrag;
		rb.angularDrag = defaultPlayerAngularDrag;
		rb.gravityScale = gravityScale;

		return rb;
	}

	/* Creates a camera to follow the player. */
	GameObject CreatePlayerCamera (GameObject followObject)
	{
		// setup camera position
		GameObject playerCamera = new GameObject ();
		playerCamera.name = "(construct)FollowCamera: " + followObject.name;
		Vector3 pos = followObject.transform.position;
		pos.z = cameraZ;
		playerCamera.transform.position = pos;

		// add camera
		playerCamera.AddComponent<Camera> ();
		// Add camera controller script
		playerCamera.AddComponent<PlayerCamera> ();
		PlayerCamera controller = playerCamera.GetComponent<PlayerCamera> ();
		controller.Camera = playerCamera.GetComponent<Camera> ();
		controller.player = followObject.transform;

		return playerCamera;
	}

	/* Creates networked player. */
	GameObject ConstructNetworkPlayer (PhotonPlayer owner, int trackerID, int controllerID)
	{
		GameObject player = Instantiate (networkPlayerModule) as GameObject;
		player.name = "(construct)player " + owner.ToString ();
		player.transform.position = new Vector3 (0,0,playerSpawnZ);

		// add rigid body
		player.AddComponent<Rigidbody2D> ();
		Rigidbody2D rb = player.GetComponent<Rigidbody2D> ();
		rb.mass = defaultPlayerMass;
		rb.drag = defaultPlayerDrag;
		rb.angularDrag = defaultPlayerAngularDrag;
		rb.gravityScale = gravityScale;

		// add photon views
		player.AddComponent<PhotonView> ();
		player.AddComponent<PhotonView> ();
		PhotonView[] views = player.GetComponents<PhotonView> ();

		// setup status tracker
		player.AddComponent<ObjectStatusController> ();
		ObjectStatusController statusController = player.GetComponent<ObjectStatusController> ();
		statusController.view = views[0];
		statusController.StatusControllerID = trackerID;
		statusController.RB = rb;
		// setup view to use the tracker
		views[0].observed = statusController;
		views[0].synchronization = ViewSynchronization.UnreliableOnChange;

		// setup controller
		player.AddComponent<NetworkPlayerController> ();
		NetworkPlayerController controller = player.GetComponent<NetworkPlayerController> ();
		controller.Owner = owner;
		controller.View = views[1];
		controller.ViewID = controllerID;
		controller.RB = rb;
		// setup view to observe the controller
		views[1].observed = controller;
		views[1].synchronization = ViewSynchronization.UnreliableOnChange;

		return player;
	}
}
