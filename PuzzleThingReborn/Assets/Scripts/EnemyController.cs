using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State { SEARCH, LOOK, CHASE, HUNT };

public class EnemyController : MonoBehaviour
{
    public State current_state;

    public GameObject enemy;
    public GameObject enemy_parent;

    Transform target;

    public List<Vector3> patrol_positions;
    int current_position;

    NavMeshAgent agent;

    public float reverse_chance = 0.5f;
    bool reversed = false;

    public float look_around_chance = 0.5f;

    public float turn_speed = 5.0f;
    public float speed;

    public GameObject player;

    public AudioSource[] spotted;
    public AudioSource searching;
    public AudioSource return_to_walking;

    public float speach_chance = 1.0f;

    // Use this for initialization
    void Start ()
    {
        current_position = 0;
         
        current_state = State.SEARCH;

        Transform[] children = GetComponentsInChildren<Transform>();

        foreach(Transform t in children)
        {
            if(t.tag == "Route")
            {
                patrol_positions.Add(t.position);
                //Destroy(t.gameObject);
            }
            else if(t.tag == "Enemy")
            {
                enemy = t.gameObject;
            }
            else if(t.tag == "EnemyParent")
            {
                enemy_parent = t.gameObject;
            }
           
        }

        agent = enemy.GetComponent<NavMeshAgent>();

        agent.speed = speed;

       
        agent.destination = patrol_positions[current_position];
        

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Captured()
    {
        Debug.Log("sup");
        current_state = State.LOOK;
        StartCoroutine(LookAround());
    }

    bool GetSpeachChance()
    {
        if(Random.Range(0.0f, 1.0f) < speach_chance)
        {
            return true;
        }

        return false;
    }

    IEnumerator LookAround()
    {
        if (GetSpeachChance() && searching.isPlaying == false)
        {
            searching.Play();
        }

        current_state = State.LOOK;       
        float vel = agent.speed;

        agent.speed = 0.0f;

        float time = 0.0f;

        while (time < 4.0f && current_state != State.CHASE)
        {
            time += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(4.0f);

        agent.speed = speed;

        //if(current_state == State.CHASE)
        //{
        //    agent.destination = patrol_positions[current_position];
        //    current_state = State.SEARCH;

        //    Debug.Log(agent.remainingDistance);
        //}

        current_state = State.SEARCH;

        if (GetSpeachChance() && return_to_walking.isPlaying == false)
        {
            return_to_walking.Play();
        }


        yield return null;
    }
    void NextPosition()
    {
        if(Random.Range(0.0f, 1.0f) < reverse_chance)
        {
            reversed = !reversed;
        }    

        if(reversed)
        {
            current_position--;
            if (current_position < 0)
            {
                current_position = patrol_positions.Count - 1;
            }
        }
        else
        {
            current_position++;
            if (current_position >= patrol_positions.Count)
            {
                current_position = 0;
            }
        }
        
        enemy.GetComponent<NavMeshAgent>().destination = patrol_positions[current_position];

        if (Random.Range(0.0f, 1.0f) <= look_around_chance)
        {
            StartCoroutine(LookAround());
            //current_state = State.LOOK;
        }
    }

    public void TargetSpotted(GameObject target_loc)
    {
        if (GetSpeachChance())
        {
            bool talk = true;
            foreach (AudioSource a in spotted)
            {
                if(a.isPlaying)
                {
                    talk = false;
                    break;
                }
            }

            if (talk)
            { spotted[Random.Range(0, spotted.Length)].Play(); }
        }

        target = target_loc.transform;

        if (current_state != State.HUNT)
        {
            current_state = State.CHASE;
        }

        agent.destination = target.position;
        enemy.transform.LookAt(target);    
    }

    public void StartHunting()
    {
        current_state = State.HUNT;
    }

	// Update is called once per frame
	void Update ()
    {
        
        enemy_parent.GetComponent<EnemyVision>().Chasing(current_state);
        switch (current_state)
        {
            case State.SEARCH:
                if (agent.remainingDistance < 0.2f)
                {
                    NextPosition();
                }
                break;
            case State.CHASE:
                if (agent.remainingDistance < 0.2f)
                {
                    StartCoroutine(LookAround());

                    //current_state = State.LOOK;
                }
                break;
            case State.LOOK:
                enemy.transform.Rotate(Vector3.up * Time.deltaTime * 90.0f, Space.World);

                break;
            case State.HUNT:
                {
                    agent.destination = player.transform.position;
                    break;
                }
        }
        
        
	}
}
