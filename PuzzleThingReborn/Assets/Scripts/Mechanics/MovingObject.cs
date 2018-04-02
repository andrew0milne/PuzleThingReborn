using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour 
{
	public Transform end_pos_obj;
	public Transform[] targets;
	public float speed = 1.0f;
	public float wait_time = 2.0f;
	public bool goes_back = true;
	public bool one_time_move = false;

	Vector3 start_pos;
	Vector3 end_pos;

	public bool is_active = false;

	protected virtual void Start()
	{
		start_pos = transform.position;

		end_pos = end_pos_obj.position;
		end_pos_obj.gameObject.SetActive (false);
	}

	public IEnumerator Move()
	{
		float t = 0;

		while (t <= 1.1f) 
		{
			transform.position = Vector3.Lerp (start_pos, end_pos, t);
			t += Time.deltaTime * speed;
			yield return null;
		}

		if (goes_back) 
		{
			StartCoroutine (MoveBack ());
		}

		if (!one_time_move) 
		{
			is_active = false;
		}

		yield return null;
	}

	public IEnumerator MoveBack()
	{
		yield return new WaitForSeconds (wait_time);

		float t = 1.0f;

		while (t >= -0.1f) 
		{
			transform.position = Vector3.Lerp (start_pos, end_pos, t);
			t -= Time.deltaTime * speed;
			yield return null;
		}

		yield return null;
	}

	void Deactivate()
	{
		StartCoroutine (MoveBack ());
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
