using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animator_controller : MonoBehaviour
{

    Animator anim;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();

        anim.Play("Rifle Idle", 0, Random.Range(0.0f, 5.0f));
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
