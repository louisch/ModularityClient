using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed;

	Rigidbody rb;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		float xForce = Input.GetAxis ("Horizontal");
		float zForce = Input.GetAxis ("Vertical");

		float rTorque = Input.GetAxis ("Rotate") * speed;

//		Vector3 force = new Vector3(xForce, 0, zForce);
//		force.Normalize ();

		rb.AddForce (transform.forward * zForce * speed);
		rb.AddForce (transform.right * xForce * speed);
		rb.AddTorque (0, rTorque, 0);

		// normalise input vecor
//		Vector3 moveBy = new Vector3 (xForce,0,zForce).normalized;
		// compute movement delta vector (frame independent)
//		moveBy = new Vector3(moveBy.x * speed * Time.fixedDeltaTime, 0, moveBy.z * speed * Time.fixedDeltaTime);
//		rb.MovePosition(gameObject.transform.position + moveBy);
//		rb.AddForce(moveBy);
	}

}
