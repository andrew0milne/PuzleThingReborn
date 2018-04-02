using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour 
{

	public Text display;

	public string answer = "";

	public Transform[] targets;

	public float wait_time = 1.0f;

	bool display_message = false;

	// Use this for initialization
	void Start () 
	{
		display.text = "";
		display.fontSize = 300;
	}

	IEnumerator DisplayText(string message)
	{
		display.fontSize = 166;
		display.text = message;

		yield return new WaitForSeconds (wait_time);

		display.text = "";
		display.fontSize = 300;

		display_message = false;
		yield return null;
	}

	void ButtonPressed(int input)
	{
		if (!display_message) 
		{
			if (input >= 0) 
			{
				if (display.text.Length < 13) 
				{
					display.text += input;
				}
			} 
			else if (input == -1) 
			{
				if (display.text.Length > 0) 
				{
					display.text = display.text.Substring (0, display.text.Length - 1);
				}
			} 
			else if (input == -2) 
			{
				display_message = true;
				if (display.text == answer) 
				{
					StartCoroutine (DisplayText ("PASSWORD CORRECT"));

					foreach (Transform t in targets) 
					{
						t.SendMessage ("Activate");
					}
				} 
				else 
				{
					StartCoroutine (DisplayText ("ERROR: \nPASSWORD INCORRECT"));
				}
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
