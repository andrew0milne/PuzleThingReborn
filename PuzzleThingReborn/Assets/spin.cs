using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    Rigidbody rb;

    public Vector3 angular_velocity;
    public Vector3 linear_velocity;

    // Use this for initialization
    void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = angular_velocity;
        rb.velocity = linear_velocity;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
