using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EngineModule : MonoBehaviour
{

    // How much force this engine exerts when on
    [SerializeField]
    private float engineForce = 0.2f;

    private Rigidbody2D rigidBody;
    private bool engineOn = false;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (engineOn) {
            rigidBody.AddForce(engineForce * (new Vector2(transform.right.x, transform.right.y)).normalized);
        }
    }

    public void ToggleEngine() {
        engineOn = !engineOn;
    }
}
