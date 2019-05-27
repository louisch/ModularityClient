using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(FixedJoint2D))]
public class ModuleJoint : MonoBehaviour
{

    private Rigidbody2D rigidbody;
    private FixedJoint2D fixedJoint;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        fixedJoint = GetComponent<FixedJoint2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
