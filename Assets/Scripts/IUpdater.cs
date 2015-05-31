using UnityEngine;

interface IUpdater {
	// accessors necessary for other classes
	int ViewID {get; set;}
	PhotonView View {get;}
	PhotonPlayer Owner {get; set;}
	GameObject GameObject {get;}
	Rigidbody BodyDouble {get; set;}

}
