using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour 
{
	public int current_level = 0;

	public GameObject[] levels;
	Vector3[] levels_pos;
	public GameObject elevator;

	public GameObject[] doors;

	// Use this for initialization
	void Start () 
	{
		levels_pos = new Vector3[levels.Length];
		for(int i = 0; i < levels.Length; i++)
		{
			levels_pos [i] = levels [i].transform.position;
			levels [i].SetActive (false);
		}
		
	}

	void ButtonPressed(int num )
	{
		if (elevator.GetComponent<MovingObject> ().is_active == false) 
		{
			int new_num = num - 1;

			if (num == -1) 
			{
				//doors [current_level].SendMessage ("Activate");
			} else if (new_num != current_level) 
			{
				elevator.SendMessage ("UpdateStartPos", levels_pos [current_level]);
				elevator.SendMessage ("UpdateEndPos", levels_pos [new_num]);
				elevator.SendMessage ("Activate");
				current_level = new_num;
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
