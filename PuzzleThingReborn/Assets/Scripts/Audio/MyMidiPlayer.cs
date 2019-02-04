using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMidiPlayer : MonoBehaviour
{

    private double nextTick = 0.0f;

    public GameObject midi_reader;
    MidiReader reader_script;

    List<DependHolder> freq_dist;

    MidiHolder note;
    MidiHolder last_note;

    public GameObject midi_player;
    MIDIPlayer midi_play;

    public  float bpm = 140.0f;
    private float bpmInSeconds;

    public GameObject music_controller;

    bool playing = false;

    // Use this for initialization
    void Start ()
    {
        reader_script = midi_reader.GetComponent<MidiReader>();
        reader_script.ReadInMidi();
    }

    void Init(double b)
    {
        bpm = MusicController.instance.bpm;

        //reader_script.CleanUp(0);
        freq_dist = reader_script.FreqDistribution();

        note = reader_script.GetFirstNote();
        last_note = note;
        last_note.pitch = -1;

        //double startTick = AudioSettings.dspTime;
        nextTick = b;

        playing = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (playing)
        {
            if (AudioSettings.dspTime >= nextTick)
            {
                if (last_note.pitch != -1)
                {
                    midi_player.GetComponent<MIDIPlayer>().NoteOff(last_note.pitch);
                }


                if (note.pitch != -1)
                {
                    midi_player.GetComponent<MIDIPlayer>().NoteOn(note.pitch);
                }

                last_note = note;


                note = reader_script.GetNote(freq_dist, last_note);

                //Debug.Log(note.length);

                nextTick += (60.0f / (MusicController.instance.bpm / note.length));
            }
        }
    }
}
