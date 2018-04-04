﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class BeamGun : MonoBehaviour 
{
	// VECTOR PS

	public GameObject beam_target_parent;
	public GameObject[] beam_targets;

	Transform current_target;

	public GameObject bullet_prefab;

	public Transform begin_pos, start_pos, mid_pos, end_pos;

	public float speed = 1.0f;

	public int max_line_points = 30;
	int bullet_total = 0;

	bool active = false;

	LineRenderer beam;

	// Use this for initialization
	void Start () 
	{
		beam = GetComponent<LineRenderer> ();
		beam.positionCount = max_line_points;

		beam_targets =  GameObject.FindGameObjectsWithTag ("BeamTarget"); //beam_target_parent.GetComponentsInChildren<Transform> ();
	}

	bool CheckRay(Vector3 pos_1, Vector3 pos_2)
	{
		RaycastHit hit;
		bool has_hit = Physics.Raycast (pos_1, pos_2 - pos_1, out hit, Vector3.Distance(pos_1, pos_2));


		if (has_hit) 
		{
			if (hit.collider.isTrigger) 
			{
				return false;
			}
		}

		return has_hit;
	}

	bool CheckBeamCollision(Vector3[] beam)
	{
		for(int i = 1; i < max_line_points; i++)
		{
			
			if (CheckRay (beam[i - 1], beam[i])) 
			{
				return true;
			}

		}
		return false;
	}

	Vector3[] GetBeam()
	{
		Vector3[] beam_points = new Vector3 [max_line_points];
		float t = 0.0f;

		for (int i = 0; i < 3; i++) 
		{
			beam_points[i] = Vector3.Lerp (begin_pos.position, start_pos.position, t);
//			if (i > 0) 
//			{
//				if (CheckRay (beam_points[i - 1],beam_points[i])) 
//				{
//					active = false;
//					break;
//				}
//			}
			t += 1.0f / 3.0f;
		}

		t = 0.0f;

		for (int i = 3; i < max_line_points; i++) 
		{
			beam_points[i] = 
				Vector3.Lerp (
					Vector3.Lerp (start_pos.position, mid_pos.position, t), 
					Vector3.Lerp (mid_pos.position, current_target.position, t), t);
//			if (CheckRay (beam_points[i - 1],beam_points[i])) 
//			{
//				active = false;
//				break;
//			}

			t += 1.0f / (max_line_points - 3);
		}

		return beam_points;
	}

	IEnumerator Shoot()
	{
		while (active) 
		{
			if (CheckBeamCollision (GetBeam()))
			{
				active = false;
				break;
			}

			beam.SetPositions(GetBeam ());

			yield return new WaitForSeconds (speed);
		}

		Deactivate ();

		yield return null;
	}

	bool FindTarget()
	{
		Vector2 mid_screen = new Vector2 (0.5f, 0.5f);

		Transform target = beam_targets [1].transform;

		float closest_dist = Vector2.Distance (mid_screen, Camera.main.WorldToViewportPoint (target.position));

		foreach (GameObject g in beam_targets) 
		{
			if (g.tag == "BeamTarget") 
			{
				Vector2 temp_pos = Camera.main.WorldToViewportPoint (g.transform.position);

				float new_dist = Vector2.Distance (mid_screen, Camera.main.WorldToViewportPoint (g.transform.position));

				if (new_dist < closest_dist) 
				{
					target = g.transform;
					closest_dist = new_dist;
				}
			}
		}

		current_target = target;

		Vector2 pos = Camera.main.WorldToViewportPoint (target.position);

		if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1) 
		{
			return false;
		}

		Vector3[] temp_points = GetBeam ();
		if (CheckBeamCollision (temp_points)) 
		{
			return false;
		}

		return true;
	}

	void Activate()
	{
		active = true;

		if (FindTarget ()) 
		{
			current_target.SendMessage ("Activate");
			StartCoroutine (Shoot ());
		}
	}

	void Deactivate()
	{
		active = false;

		current_target.SendMessage ("Deactivate");

		for (int i = 0; i < max_line_points; i++) 
		{
			beam.SetPosition (i, begin_pos.position);
		}

	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
