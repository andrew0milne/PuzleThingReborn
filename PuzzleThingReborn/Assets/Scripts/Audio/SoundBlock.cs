using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NAudio.Wave.SampleProviders;
using NAudio.Wave;

public class SoundBlock : MonoBehaviour
{

    public int note;

    //SignalGenerator sig_gen;
    //WaveOutEvent out_event;

    // Use this for initialization
    void Start ()
    {
        
    }

    public void Init(int n)
    {
        note = n;
        
        //sig_gen = new SignalGenerator();
        //sig_gen.Gain = 0.3f;
        //out_event = new WaveOutEvent();

        //sig_gen.Frequency = freq;
        //out_event.Init(sig_gen);
        ////Debug.Log(frequency);

    }


    // Update is called once per frame
    void Update ()
    {
		
	}
}
