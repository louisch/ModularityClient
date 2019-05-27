using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EngineModule : MonoBehaviour
{

    // How much force this engine exerts when on
    [SerializeField]
    private float engineForce = 0.2f;

    private Rigidbody2D c_Rigidbody;

    private bool m_EngineOn = false;

    // Start is called before the first frame update
    void Start() {
        c_Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (m_EngineOn) {
            c_Rigidbody.AddForce(engineForce * (new Vector2(transform.right.x, transform.right.y)).normalized);
        }
    }

    public void ToggleEngine() {
        m_EngineOn = !m_EngineOn;
    }
}
