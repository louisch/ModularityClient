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
		float xForce = Input.GetAxis ("Rotate") * speed;
		float zForce = Input.GetAxis ("Vertical") * speed;

		float rTorque = Input.GetAxis ("Horizontal") * speed;

//		Vector3 force = new Vector3(xForce, 0, zForce);
//		force.Normalize ();

		rb.AddForce (transform.forward * zForce);
		rb.AddForce (transform.right * xForce);
		rb.AddTorque (0, rTorque, 0);
//		Debug.Log ("Adding forces" + xForce + zForce);
	}

}
