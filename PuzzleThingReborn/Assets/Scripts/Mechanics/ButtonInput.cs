using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInput : MovingObject 
{

	public GameObject input_target;
	public int input_num;

	public Text text;

	protected override void Start()
	{
		base.Start ();

		if (input_num >= 0) 
		{
			text.text = "" + input_num;
		} 
		else if (input_num == -1) 
		{
			text.text = "<--";
		} 
		else if (input_num == -2) 
		{
			text.text = "ENT";
		}
	}

	protected override void Activate()
	{
		if(!is_active)
		{
			base.Activate ();
			is_active = true;
			input_target.SendMessage ("ButtonPressed", input_num);
		}
	}
}
