using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public GameObject player;
    public GameObject pickUps;

    public GameObject[] lasers;

    int max_pickups = 0;

    int score = 0;

    public float lockdown_start = 0.5f;

    bool LOCKDOWN = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        Transform[] temp = pickUps.GetComponentsInChildren<Transform>();

        max_pickups = temp.Length - 1;

        Debug.Log(max_pickups);// max_pickups);

        lasers = GameObject.FindGameObjectsWithTag("Laser");

        Debug.Log("Laser count: " + lasers.Length);

        Debug.Log("LockDown at " + max_pickups * lockdown_start);
	}

    public void UpdateScore(int num)
    {
        score = num;

        if(score >= max_pickups * lockdown_start && !LOCKDOWN)
        {
            LockDown();
        }
    }

    void LockDown()
    {
        Debug.Log("LOCKDOWN");

        LOCKDOWN = true;

        foreach (GameObject l in lasers)
        {
            l.SendMessage("LockDown");
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
