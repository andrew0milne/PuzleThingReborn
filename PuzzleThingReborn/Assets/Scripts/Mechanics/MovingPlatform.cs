using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MovingObject 
{
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") 
		{
			col.gameObject.transform.parent = gameObject.transform;
		}
	}


	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player") 
		{
			if(col.gameObject.transform.parent == gameObject.transform)
			{
				col.gameObject.transform.parent = null;
			}
		}
	}
}
