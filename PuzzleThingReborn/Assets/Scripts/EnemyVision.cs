﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{

    public GameObject enemy;
    public Light torch;

    public bool update_torch = true;

	// Use this for initialization
	void Start ()
    {
        //torch = GetComponent<Light>();
	}
    
    public void Chasing(State state)
    {
        if (update_torch)
        {
            switch (state)
            {
                case State.SEARCH:
                    torch.color = Color.cyan;
                    break;
                case State.CHASE:
                    torch.color = Color.red;
                    break;
                case State.LOOK:
                    torch.color = Color.yellow;
                    break;
            }
        }
    }

    void CheckTarget(Collider col)
    {
        if (col.tag == "Player")
        {
            RaycastHit hit;

            Vector3 dir = col.transform.position - enemy.GetComponent<EnemyController>().enemy.transform.position;

            if (Physics.Raycast(enemy.GetComponent<EnemyController>().enemy.transform.position, dir * 1000000.0f, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Player")
                {
                    enemy.GetComponent<EnemyController>().TargetSpotted(col.gameObject);
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        CheckTarget(col);
    }
}
