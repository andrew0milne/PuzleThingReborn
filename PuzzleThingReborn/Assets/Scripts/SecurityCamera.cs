using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{

    public float rotate_amount = 45.0f;
    public float rotate_speed = 1.0f;

    public bool clockwise = true;

    Quaternion left_rot, right_rot;

    bool rotating = true;

    float time = 0.0f;

    public Transform axis;

	// Use this for initialization
	void Start ()
    {       
        left_rot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, rotate_amount, 0.0f));
        right_rot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, -rotate_amount, 0.0f));

        rotate_speed *= Random.Range(0.9f, 1.1f);

       
    }
	
    

	// Update is called once per frame
	void Update ()
    {
        if (rotating)
        {
            transform.rotation = Quaternion.Lerp(left_rot, right_rot, time);

            if (time >= 1.1f)
            {
                clockwise = false;
            }
            else if (time <= -0.1f)
            {
                clockwise = true;
            }

            if (clockwise)
            {
                time += Time.deltaTime * rotate_speed;
            }
            else
            {
                time -= Time.deltaTime * rotate_speed;
            }
        }
	}
}
