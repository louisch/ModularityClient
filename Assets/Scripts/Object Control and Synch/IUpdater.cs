using UnityEngine;

/**
* Interface for object updater classes.
* Must be implemented for any script that intends to synchronise objects with the server.
*/
interface IUpdater {
	/* Accessors. (could you tell =D) */
	int ViewID {get;}
	PhotonView View {get;}
	PhotonPlayer Owner {get;}
	GameObject gameObject {get;}

	/**
	* This method must be called as soon as possible after initialising the object.
	* Sets up fields like ownership information, viewId (essential for synch)
	* and other object-specific things (see implementations for details).
	*/
	void SetupSpawn(PhotonPlayer owner, int viewID);
}
