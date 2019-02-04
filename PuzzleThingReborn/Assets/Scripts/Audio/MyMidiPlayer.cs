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

    public  float bpm = 140.0f;
    private float bpmInSeconds;

    // Use this for initialization
    void Start ()
    {
       // midi_player;// = new MIDIPlayer();
        //midi_player.

        //reader_script = midi_reader.GetComponent<MidiReader>();

        //reader_script.ReadInMidi();
        //reader_script.CleanUp(0);
        //freq_dist = reader_script.FreqDistribution();

        //note = reader_script.GetFirstNote();
        //last_note = note;
        //last_note.pitch = -1;

        //double startTick = AudioSettings.dspTime;
        //nextTick = startTick + (60.0f / (bpm / 2.0f));

        //bpmInSeconds = (60.0f / bpm);
    }

    void InitBPM(int b)
    {
        bpm = b;

        reader_script = midi_reader.GetComponent<MidiReader>();

        reader_script.ReadInMidi();
        //reader_script.CleanUp(0);
        freq_dist = reader_script.FreqDistribution();

        note = reader_script.GetFirstNote();
        last_note = note;
        last_note.pitch = -1;

        double startTick = AudioSettings.dspTime;
        nextTick = startTick + (60.0f / (bpm / 2.0f));

        bpmInSeconds = (60.0f / bpm);
    }
	
	// Update is called once per frame
	void Update ()
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

            //while(note == null)
            //{
            //    note = reader_script.GetNote(freq_dist, last_note);
            //}

            nextTick += (60.0f / (bpm / note.length));
            //count++;
            //if (count >= beats.Length)
            //{
            //    count = 0;
            //    if (new_notes_each_repeat)
            //    {
            //        beats = GetNotes(note_count);
            //    }
            //}
        }
    }
}
