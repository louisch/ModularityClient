using UnityEngine;
using System.Collections;

public class Railgun : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
    }

	// Update is called once per frame
	void Update () {}

    void OnTriggerStay2D (Collider2D other)
    {
        Rigidbody2D otherRigidBody = other.GetComponentInParent<Rigidbody2D> ();
        otherRigidBody.AddForce (transform.up * 10);
    }
}
