using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    LineRenderer line;

    public GameObject[] pos;

    public int laser_res;

    ParticleSystem ps;

    Vector3[] temp_beam;

    bool active = false;

    public float activate_time = 5.0f;
    public Collider stop;

    bool always_on = false;

    AudioSource audio;
    public bool play_sound = false;

    // Use this for initialization
    void Start ()
    {
        line = GetComponent<LineRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();

        audio = GetComponent<AudioSource>();

        line.positionCount = laser_res + 1;

        Vector3 start = pos[0].transform.position;
        Vector3 end = pos[1].transform.position;

        float t = 0;

        for (int i = 0; i < laser_res + 1; i ++)
        {
            line.SetPosition(i, Vector3.Lerp(start, end, t));

            t += 1.0f / (laser_res);
        }

        //line.SetPosition(0, pos[0].transform.position);
        //line.SetPosition(1, pos[1].transform.position);


        temp_beam = new Vector3[line.positionCount];

        line.enabled = false;
        stop.enabled = false;
        ps.Stop();

        //Activate(active);
    }

    void SetLineWidth(float num)
    {
        line.endWidth = num;
        line.startWidth = num;
    }

    void LockDown()
    {
        always_on = true;
        Activate();
    }

    IEnumerator LaserOn()
    {

        line.enabled = true;
        stop.enabled = true;
        ps.Play();
        if (play_sound)
        { audio.Play(); }

        if (!always_on)
        {
            yield return new WaitForSeconds(activate_time);

            line.enabled = false;
            stop.enabled = false;
            ps.Stop();
            if (play_sound)
            { audio.Stop(); }
        }

        yield return null;
    }

    void Activate()
    {

        if (line.enabled == false)
        {

            StartCoroutine(LaserOn());
        }
        
        //line.enabled = b;
        //if (b)
        //{
        //    ps.Play();       
        //}
        //else
        //{
        //    ps.Stop();
        //}

        //active = b;
    }

    // Update is called once per frame
    void Update ()
    {
        if (line.enabled)
        {
            line.GetPositions(temp_beam);
            ps.transform.position = temp_beam[(int)Random.Range(0, laser_res - 1)];
            SetLineWidth(Random.Range(0.075f, 0.125f));
        }
    }
}
