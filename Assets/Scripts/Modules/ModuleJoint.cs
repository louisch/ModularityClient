using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(FixedJoint2D))]
public class ModuleJoint : MonoBehaviour
{

    private Rigidbody2D c_RigidBody;
    private FixedJoint2D c_FixedJoint;

    // Start is called before the first frame update
    void Start()
    {
        c_RigidBody = GetComponent<Rigidbody2D>();
        c_FixedJoint = GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
