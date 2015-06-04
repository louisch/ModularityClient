using UnityEngine;

public interface IController {
	PhotonPlayer Owner {set;}
	PhotonView View {set;}
	int ControllerID {set;}
	Rigidbody2D Rb {get;set;}
	Rigidbody2D Bodydouble {set;}
	PlayerCamera Camera {set;}
}
