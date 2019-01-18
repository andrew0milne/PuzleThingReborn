using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGrid : MonoBehaviour
{

    public Laser[] grid;

    public bool music_on = false;

    // Use this for initialization
    void Start()
    {
        grid = GetComponentsInChildren<Laser>();
    }

    public void ToggleLasers(int num)
    {
        if (music_on)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i].SendMessage("Activate", false);
            }

            if (num != -1)
            {
                grid[num].SendMessage("Activate", true);
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
