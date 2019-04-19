using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLight : MonoBehaviour
{

    public GameObject spinner;
    public Light[] lights;

    public int state = 0;

    public Color warning;
    public Color hunting;

    Color color;

    public float rotation_speed = 150.0f;
    public float hunting_speed_increase = 1.2f;

    public void UpdateState()
    {
        state++;

        if(state == 1)
        {
            color = warning;
        }
        else if (state == 2)
        {
            color = hunting;
            rotation_speed *= hunting_speed_increase;
        }

        foreach (Light l in lights)
        {
            l.enabled = true;
            l.color = color;
        }

        spinner.GetComponent<Renderer>().material.SetColor("_EmissionColor", color * 1.8f);

    }

	// Update is called once per frame
	void Update ()
    {
		if(state != 0)
        {
            spinner.transform.Rotate(Vector3.up * Time.deltaTime * rotation_speed, Space.Self);
        }
	}
}



