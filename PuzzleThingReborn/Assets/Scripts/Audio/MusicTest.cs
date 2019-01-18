﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTest : MonoBehaviour
{

    private float bpm = 140.0f;
    private float bpmInSeconds;
    public float noteLength = 1.0f;

    private double nextTick = 0.0f;

    private double sampleRate = 0.0f;

    public AudioSource audio;

    public bool[] beats;
    int count = 0;

    //public GameObject cube;

    void Start()
    {
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick;// * sampleRate;
        bpmInSeconds = (60.0f / (bpm * noteLength)) / 2.0f;

        audio = GetComponent<AudioSource>();

        //beats = new bool[8];
    }

    void InitBPM(float bpm_)
    {
        bpm = bpm_;
    }

    private void Update()
    {
        if (AudioSettings.dspTime >= nextTick)
        {
            if (beats[count] == true)
            {
                audio.Play();
                //cube.GetComponent<Renderer>().material.color = Color.red;
            }
            else
            {
                //cube.GetComponent<Renderer>().material.color = Color.black;
            }
            nextTick += (60.0f / (bpm * noteLength)) / 2.0f; ;
            count++;
            if (count >= beats.Length)
            {
                count = 0;
            }
        }
    }


}