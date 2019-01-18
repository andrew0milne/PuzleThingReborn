using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWave : MonoBehaviour {

    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency1;

    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency2;

    public float sampleRate = 44000;
    public float waveLengthInSeconds = 2.0f;

    AudioSource audioSource;
    public int timeIndex = 0;

    public int maxTimeIndex = 0;

    private float bpm = 140.0f;
    private float bpmInSeconds;
    public float noteLength = 1.0f;

    private double nextTick = 0.0f;

    public float pitch;

    public int note_count;

    public string[] beats;
    int count = 0;

    //public GameObject cube;

    //public float num;

    public GameObject MusicController;
    MusicController controller;
    public bool new_notes_each_repeat = false;

    LaserGrid grid;

    private string[] notes = new string[13] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "AA" };

    int[] scale;
    int[] num;

    public float rest_spawn_rate;

    public float volume = 0.2f;

    void Start()
    {
        MusicController = GameObject.FindGameObjectWithTag("MusicController");
        controller = MusicController.GetComponent<MusicController>();

        scale = controller.GetScale();

        beats = GetNotes(note_count);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically

        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick;// * sampleRate;
        bpmInSeconds = (60.0f / (bpm * noteLength)) / 2.0f;

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
        //audioSource.pitch = pitch;

        audioSource.Play();

        grid = GetComponent<LaserGrid>();
    }

    void InitBPM(float bpm_)
    {
        bpm = bpm_;
    }

    public string[] GetNotes(int note_count)
    {
        string[] beats = new string[note_count];

        int[] current_scale = controller.GetScale();

        num = new int[note_count];

        for (int i = 0; i < note_count; i++)
        {
            // If true make a note
            if (Random.Range(0.0f, 1.0f) >= rest_spawn_rate)
            {
                num[i] = (int)Random.Range(0, 7);
                beats[i] = notes[current_scale[num[i]]];
            }
            else
            {
                num[i] = -1;
                beats[i] = "";
            }
        }

        return beats;
    }

    float GetFreq(string name)
    {
        switch(name)
        {
            case "A":
                return 110.0f;
            case "A#":
                return 116.54f;
            case "B":
                return 123.47f;
            case "C":
                return 130.81f;
            case "C#":
                return 138.59f;
            case "D":
                return 146.83f;
            case "D#":
                return 155.56f;
            case "E":
                return 164.81f;
            case "F":
                return 174.61f;
            case "F#":
                return 185.0f;
            case "G":
                return 196.0f;
            case "G#":
                return 207.65f;
            case "AA":
                return 220.0f;

        }

        return 0.0f;
    }

    void Update()
    {
        if (AudioSettings.dspTime >= nextTick)
        {
            grid.ToggleLasers(num[count]);
            if (beats[count] != "")
            {
                frequency1 = GetFreq(beats[count]);
                frequency2 = frequency1;

                timeIndex = 0;  //resets timer before playing sound

                
            }
            else
            {
                frequency1 = 0.0f;
                frequency2 = 0.0f;
            }

            nextTick += (60.0f / (bpm * noteLength)) / 2.0f; ;
            count++;
            if (count >= beats.Length)
            {
                count = 0;
                if(new_notes_each_repeat)
                {
                    beats = GetNotes(note_count);
                }
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            data[i] = CreateSine(timeIndex, frequency1, sampleRate);

            if (channels == 2)
            {
                data[i + 1] = CreateSine(timeIndex, frequency2, sampleRate);
            }

            timeIndex++;

            if (timeIndex > maxTimeIndex)
            {
                maxTimeIndex = timeIndex;
            }

            //if timeIndex gets too big, reset it to 0
            if (timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * pitch * frequency / sampleRate);
    }
}
