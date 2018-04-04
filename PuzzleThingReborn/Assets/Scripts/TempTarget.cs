using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTarget : MonoBehaviour 
{

	Light light;
	Renderer rend;

	public Transform[] targets;

	// Use this for initialization
	void Start () 
	{
		
	}

	void Activate()
	{
		GetComponent<Light> ().enabled = true;
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.red);

		foreach (Transform t in targets) 
		{
			t.SendMessage ("Activate");
		}
	}

	void Deactivate()
	{
		GetComponent<Light> ().enabled = false;
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.black);
	}


	// Update is called once per frame
	void Update () 
	{
		
	}
}
