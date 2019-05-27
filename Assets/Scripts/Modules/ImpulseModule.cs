using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class ImpulseModule : MonoBehaviour {

    [SerializeField]
    private float magnitudeOfForce = 3.0f;

    private Rigidbody2D c_RigidBody;

    // Start is called before the first frame update
    void Start() {
        c_RigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
    }

    public void ApplyImpulse() {
        c_RigidBody.AddForce(magnitudeOfForce * transform.right, ForceMode2D.Impulse);
    }
}
