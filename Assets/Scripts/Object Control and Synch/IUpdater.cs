using UnityEngine;

interface IUpdater {
	// accessors necessary for other classes
	int ViewID {get;}
	PhotonView View {get;}
	PhotonPlayer Owner {get;}
	void SetupSpawn(PhotonPlayer owner, int viewID);
	void Despawn();
}
