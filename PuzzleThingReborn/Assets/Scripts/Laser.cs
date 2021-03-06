﻿using System.Collections;
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

    bool active = true;

    // Use this for initialization
    void Start ()
    {
        line = GetComponent<LineRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();

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

        Activate(active);
    }

    void SetLineWidth(float num)
    {
        line.endWidth = num;
        line.startWidth = num;
    }

    void Activate(bool b)
    {
        line.enabled = b;
        if (b)
        {
            ps.Play();       
        }
        else
        {
            ps.Stop();
        }

        active = b;
    }

    // Update is called once per frame
    void Update ()
    {
        if (active)
        {
            line.GetPositions(temp_beam);
            ps.transform.position = temp_beam[(int)Random.Range(0, laser_res - 1)];
            SetLineWidth(Random.Range(0.075f, 0.125f));
        }
    }
}
