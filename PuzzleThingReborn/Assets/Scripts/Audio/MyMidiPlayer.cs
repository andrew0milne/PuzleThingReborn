﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMidiPlayer : MonoBehaviour
{

    private double nextTickMarkov = 0.0f;
    private double nextTickChords = 0.0f;

    public GameObject midi_reader;
    MidiReader reader_script;

    //List<DependHolder> freq_dist;

    MidiHolder note;
    MidiHolder last_note;
    int[] chord;

    public GameObject midi_player;
    MIDIPlayer midi_play;

    public  float bpm = 140.0f;
    private float bpmInSeconds;

    float shortest_note_length;

    bool playing = false;

    float beat = 0;
    int bar = 0;

    int num_chord = 0;
    int num_note = 0;

    public bool markov = true;
    public bool chords = true;

    int base_note = 36;

    public int markov_offset = 0;
    public int chord_offset = 0;

    List<MidiHolder>[] theme;
    int[] theme_counter;

    float note_length = 0.0f;
    float chord_length = 0.0f;

    //List<MidiHolder> melody;
    //int melody_counter = 0;

    // Use this for initialization
    void Start ()
    {
        //reader_script = midi_reader.GetComponent<MidiReader>();
        

        chord = new int[3];
    }

    void Init(double b)
    {
        bpm = MusicController.instance.bpm;
        shortest_note_length = MusicController.instance.shortest_note_length;

        //reader_script.CleanUp(0);
        //freq_dist = reader_script.FreqDistribution();

        //double startTick = AudioSettings.dspTime;
        nextTickMarkov = b;
        nextTickChords = b;

        playing = true;

        Debug.Log("hello there");

        theme = MusicController.instance.GetTheme(true, 4, 1.0f, 2.0f, 4.0f);
        //melody = MusicController.instance.GetThemeNotes(true, 4.0f);

        theme_counter = new int[2] { 0, 0 };
        


        note = theme[1][0];
        last_note = note;
        last_note.pitch[0] = -1;
    }
	
    

    // code
    void GetChord(int pitch, bool on)
    {
        if (on)
        {
            for(int i = 0; i <3; i++)
            {
                chord[i] = MusicController.instance.GetNoteInChord(pitch, i);
                chord[i] += 48;
                midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[i] + (12* chord_offset));

                
            }
            Debug.Log(MusicController.instance.GetNoteName(chord[0]) + ", " + MusicController.instance.GetNoteName(chord[1]) + ", " + MusicController.instance.GetNoteName(chord[2]));
        }
        else
        {
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[0] + (12 * chord_offset));
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[1] + (12 * chord_offset));
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[2] + (12 * chord_offset));
        }
        
    }

    void PlayScale()
    {
        if (AudioSettings.dspTime >= nextTickChords && chords)
        {
            if (note_length >= theme[0][theme_counter[0]].length)
            {
                int num = theme_counter[0] - 1;
                if (num < 0)
                {
                    num = theme[0].Count - 1;
                }



                GetChord(theme[0][num].pitch[0], false);

                //Debug.Log("root: " + theme[0][theme_counter[0]].pitch[0] );

                GetChord(theme[0][theme_counter[0]].pitch[0], true);
                //Debug.Log(theme[0][theme_counter[0]].pitch[0] + ", " + theme[0][theme_counter[0]].length);          

                note_length = 0.0f;

                theme_counter[0]++;
                if (theme_counter[0] >= theme[0].Count)
                {
                    theme_counter[0] = 0;
                }
            }

            nextTickChords += MusicController.instance.time_step; //  (60.0f / (MusicController.instance.bpm / theme[0][theme_counter[0]].length));
            note_length += MusicController.instance.shortest_note_length;
        }


        if (AudioSettings.dspTime >= nextTickMarkov && markov)
        {

            if (chord_length >= theme[1][theme_counter[1]].length)
            {
                if (last_note.pitch[0] != -1)
                {
                    for (int i = 0; i < last_note.pitch.Count; i++)
                    {
                        midi_player.GetComponent<MIDIPlayer>().NoteOff(MusicController.instance.RoundNote(last_note.pitch[i]) + (12 * markov_offset));// + MusicController.instance.GetRootNote());

                    }
                }

                if (note.pitch[0] != -1)
                {
                    for (int i = 0; i < note.pitch.Count; i++)
                    {

                        //Debug.Log(MusicController.instance.RoundNote(note.pitch[i]) + MusicController.instance.GetRootNote());
                        midi_player.GetComponent<MIDIPlayer>().NoteOn(MusicController.instance.RoundNote(note.pitch[i]) + (12 * markov_offset));// + MusicController.instance.GetRootNote());

                        //Debug.Log(MusicController.instance.GetNoteName(note.pitch[i]) + ", " + MusicController.instance.GetNoteName(MusicController.instance.RoundNote(note.pitch[i])));
                        Debug.Log(MusicController.instance.GetNoteName(MusicController.instance.RoundNote(note.pitch[i])));// + MusicController.instance.GetRootNote()));
                    }


                }

                last_note = note;

                theme_counter[1]++;
                if (theme_counter[1] >= theme[1].Count)
                {
                    theme_counter[1] = 0;
                }
                note = theme[1][theme_counter[1]];
            }

            nextTickMarkov += MusicController.instance.time_step;// (60.0f / (MusicController.instance.bpm / note.length));
            chord_length += MusicController.instance.shortest_note_length;

        }

    }

    // Update is called once per frame
    void Update ()
    {
        if (playing)
        {
            PlayScale();
            
            //if (AudioSettings.dspTime >= nextTickMarkov)
            //{
            //    if(markov)
            //    {
            //        if (last_note.pitch[0] != -1)
            //        {
            //            for (int i = 0; i < last_note.pitch.Count; i++)
            //            {
            //                midi_player.GetComponent<MIDIPlayer>().NoteOff(last_note.pitch[i]);
            //            }                        
            //        }

            //        if (note.pitch[0] != -1)
            //        {
            //            for (int i = 0; i < note.pitch.Count; i++)
            //            {
            //                midi_player.GetComponent<MIDIPlayer>().NoteOn(note.pitch[i]);                         
            //            }

                       
            //        }

            //        last_note = note;

            //        note = reader_script.GetNote(freq_dist, last_note);

            //        nextTickMarkov += (60.0f / (MusicController.instance.bpm / note.length));
                    
            //    }
            //    else
            //    {
            //        if (beat == 0.0f)
            //        {
            //            midi_player.GetComponent<MIDIPlayer>().NoteOn(MusicController.instance.GetChordRootNote(num_note) + 36);
            //            //Debug.Log(MusicController.instance.GetChordRootNote(num_note));

            //            GetChord(MusicController.instance.GetChordRootNote(num_note), false);
            //        }
            //        else if (beat == shortest_note_length * 4.0f)
            //        {
            //            midi_player.GetComponent<MIDIPlayer>().NoteOff(MusicController.instance.GetChordRootNote(num_note) + 36);

            //            num_chord = num_note;

            //            GetChord(MusicController.instance.GetChordRootNote(num_note), true);

            //            num_note = MusicController.instance.GetNextChord(num_note);
            //        }

            //        nextTickChords += MusicController.instance.time_step;
            //    }

            //    //beat += shortest_note_length;
            //    //if(beat >= 2.0f)
            //    //{
            //    //    beat = 0.0f;
            //    //    bar++;
            //    //}

            //    //nextTick += MusicController.instance.time_step;// (60.0f / (MusicController.instance.bpm / note.length));
            //}

            //if(AudioSettings.dspTime >= nextTickChords)
            //{
            //    if (beat == 0.0f)
            //    {
            //        num_chord = note.pitch[0];
            //        if (num_chord != -1)
            //        {
            //            num_chord = MusicController.instance.GetRootNote();
                        
            //        }
            //        GetChord(num_chord, true);
            //    }
            //    else if (beat == shortest_note_length )
            //    {
            //        //if (num_chord != -1)
            //        //{
            //            GetChord(num_chord, false);
            //        //}
            //    }

            //    beat += shortest_note_length;
            //    if (beat >= 2.0f)
            //    {
            //        beat = 0.0f;
            //        bar++;
            //    }

            //    nextTickChords += MusicController.instance.time_step;
            //}
        }
    }
}
