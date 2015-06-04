using UnityEngine;
using System.Collections;

public class ObjectConstructor : MonoBehaviour {
	// player variables
	public GameObject playerModule;
	public GameObject networkPlayerModule;
	public GameObject randomModule;

	public float moduleSpawnZ = 3;
	public float cameraSpawnZ = 0;

	/* Constructs a player object in game. */
	public IController ConstructPlayer (PhotonPlayer owner, int trackerID, int controllerID, Vector2 position, float rotation)
	{
		GameObject player;
		if (owner.isLocal)
		{
			player = InstantiateModule(playerModule, owner, "player");
		}
		else
		{
			player = InstantiateModule(networkPlayerModule, owner, "networkPlayer");
		}
		IController controller = AddModuleComponents (player, owner, new PilotModuleRigidbodyInfo (), trackerID, controllerID, position, rotation);

		if (owner.isLocal)
		{
			CreateBodyDouble (owner, controller);
			CreatePlayerCamera (owner, controller);
		}
		Debug.Log (controller.Rb.position);

		return controller;
	}

	GameObject InstantiateModule (GameObject prefab, PhotonPlayer owner, string nameString)
	{
		GameObject module = Instantiate<GameObject> (prefab);
		module.name = "(construct)" + nameString + " " + owner.ToString ();
		return module;
	}
	GameObject InstantiateModule (PhotonPlayer owner, string nameString)
	{
		GameObject module = new GameObject ();
		module.name = "(construct)" + nameString + " " + owner.ToString ();
		return module;
	}

	IController AddModuleComponents (GameObject module, PhotonPlayer owner, RigidbodyInfo info, int trackerID, int controllerID, Vector2 position, float rotation)
	{
		Rigidbody2D rb = AddRigidbody (module, info, position, rotation);
		AddStatusTracker (module, rb, trackerID);
		return AddController (module, owner, rb, controllerID);
	}

	Rigidbody2D AddRigidbody (GameObject module, RigidbodyInfo info, Vector2 position, float rotation)
	{
		Vector3 pos = (Vector3)position + new Vector3 (0,0,moduleSpawnZ);
		Debug.Log (pos);
		Rigidbody2D rb = module.AddComponent<Rigidbody2D> ();
		// disable ridigbody
		rb.Sleep ();
		// set up rigidbody info
		rb.mass = info.mass;
		rb.drag = info.drag;
		rb.angularDrag = info.angularDrag;
		rb.gravityScale = info.gravityScale;
		// set object's positional info
		module.transform.position = pos;
		rb.rotation = rotation;
		// re-enable rigidbody
		rb.WakeUp ();

		return rb;
	}

	ObjectStatusController AddStatusTracker (GameObject module, Rigidbody2D rb, int trackerID)
	{
		// add photon view and tracker
		PhotonView view = module.AddComponent<PhotonView> ();
		ObjectStatusController tracker = module.AddComponent<ObjectStatusController> ();
		tracker.View = view;
		tracker.Rb = rb;
		tracker.TrackerID = trackerID;

		// setup view to use the tracker
		view.observed = tracker;
		view.synchronization = ViewSynchronization.UnreliableOnChange;

		return tracker;
	}

	IController AddController (GameObject module, PhotonPlayer owner, Rigidbody2D rb, int controllerID)
	{
		PhotonView view = module.AddComponent<PhotonView> ();
		IController controller;
		if (owner.isLocal)
		{
			controller = module.AddComponent<LocalPlayerController> ();
		}
		else
		{
			controller = module.AddComponent<NetworkPlayerController> ();
		}

		controller.Owner = owner;
		controller.View = view;
		controller.ControllerID = controllerID;
		controller.Rb = rb;

		// setup view to observe the controller
		view.observed = controller as Component;
		view.synchronization = ViewSynchronization.UnreliableOnChange;

		return controller;
	}


	/* Constructs a body double for the player. */
	GameObject CreateBodyDouble (PhotonPlayer player, IController controller)
	{
		Rigidbody2D actor = controller.Rb;

		GameObject bodydouble = InstantiateModule (player, "bodydouble");
		controller.Bodydouble = AddRigidbody (bodydouble, new InfoFromRigidbody(actor), actor.position, actor.rotation);

		return bodydouble;
	}


	/* Creates a camera to follow the player. */
	GameObject CreatePlayerCamera (PhotonPlayer player, IController controller)
	{
		// setup camera position
		GameObject playerCamera = InstantiateModule (player, "playerCamera");
		Vector3 pos = controller.Rb.position;
		pos.z = cameraSpawnZ;
		playerCamera.transform.position = pos;

		// add camera
		playerCamera.AddComponent<Camera> ();
		// Add camera controller script
		playerCamera.AddComponent<PlayerCamera> ();
		PlayerCamera cameraController = playerCamera.GetComponent<PlayerCamera> ();
		cameraController.Camera = playerCamera.GetComponent<Camera> ();
		cameraController.player = controller.Rb.gameObject.transform;

		controller.Camera = cameraController;

		return playerCamera;
	}
}
