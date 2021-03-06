﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour 
{
	public Transform end_pos_obj;
	public Transform[] targets;
	public float speed = 1.0f;
	public float wait_time = 2.0f;
	public float start_wait_time = 0.0f;
	public bool goes_back = true;
	public bool one_time_move = false;

	Vector3 start_pos;
	Vector3 end_pos;

	public bool is_active = false;

    public bool clickable = true;

	float t = 0.0f;

	public Transform[] parents;
	public Transform moving_parent;
	Vector3 parent_start_location = Vector3.zero;

	protected virtual void Start()
	{
		start_pos = transform.position;

		if (end_pos_obj != null) 
		{
			end_pos = end_pos_obj.position;
			end_pos_obj.gameObject.SetActive (false);
		} 
		else 
		{
			end_pos = transform.position;
		}

		parents = GetComponentsInParent<Transform> ();

		for (int i = 1; i < parents.Length; i++) 
		{
			if (parents [i].GetComponent<MovingObject> () != null) 
			{
				moving_parent = parents [i];
				parent_start_location = moving_parent.transform.position;
				break;
			}
		}
	}

	void UpdateEndPos(Vector3 new_end_pos)
	{
		end_pos = new_end_pos;
	}

	void UpdateStartPos(Vector3 new_start_pos)
	{
		start_pos = new_start_pos;
	}

	public IEnumerator Move()
	{
		float t = 0;

		while (t <= 1.1f) 
		{
			if (moving_parent != null) 
			{
				transform.position = Vector3.Lerp (start_pos + moving_parent.position - parent_start_location, end_pos + moving_parent.position - parent_start_location, t);
			} 
			else 
			{
				transform.position = Vector3.Lerp (start_pos, end_pos, t);
			}
			t += Time.deltaTime * speed;
			yield return null;
		}

		if (goes_back) 
		{
			t = 1.0f;

			yield return new WaitForSeconds (wait_time);

			while (t >= -0.1f) 
			{
				if (moving_parent != null) 
				{
					transform.position = Vector3.Lerp (start_pos + moving_parent.position - parent_start_location, end_pos + moving_parent.position - parent_start_location, t);
				} 
				else 
				{
					transform.position = Vector3.Lerp (start_pos, end_pos, t);
				}
				t -= Time.deltaTime * speed;
				yield return null;
			}
		}

		if (!one_time_move) 
		{
			is_active = false;
		}

		yield return null;
	}

	void Deactivate()
	{
		//StartCoroutine (MoveBack ());
	}

	protected virtual void Activate()
	{
		if(!is_active)
		{
			is_active = true;

			StartCoroutine (Move ());
			foreach (Transform t in targets) 
			{
				t.SendMessage ("Activate");
			}
		}
	}
}
