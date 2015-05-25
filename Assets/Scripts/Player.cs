using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// movement vars
	public float acceleration;

	void FixedUpdate () {
		float hMove = Input.GetAxis ("Horizontal");
		float vMove = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3(hMove, 0, vMove);
		GetComponent<Rigidbody>().velocity = movement * acceleration;
	}
}
