using System.Collections;
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

    
    List<MidiHolder>[] theme;
    int[] theme_counter;

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
                midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[i]);

                Debug.Log(MusicController.instance.GetNoteName(chord[i]));
            }
            
        }
        else
        {
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[0]);
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[1]);
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[2]);
        }
        
    }

    void PlayScale()
    {
        if (AudioSettings.dspTime >= nextTickChords)
        {
            int num = theme_counter - 1;
            if (num < 0)
            {
                num = theme.Count - 1;
            }

            GetChord(theme[num].pitch[0], false);

            GetChord(theme[theme_counter].pitch[0], true);
            Debug.Log(theme[theme_counter].pitch[0] + ", " + theme[theme_counter].length);          

            nextTickChords += (60.0f / (MusicController.instance.bpm / theme[theme_counter].length));

            theme_counter++;
            if (theme_counter >= theme.Count)
            {
                theme_counter = 0;
            }
        }

        if (AudioSettings.dspTime >= nextTickMarkov)
        {

            if (last_note.pitch[0] != -1)
            {
                for (int i = 0; i < last_note.pitch.Count; i++)
                {
                    midi_player.GetComponent<MIDIPlayer>().NoteOff(MusicController.instance.RoundNote(last_note.pitch[i]) + MusicController.instance.GetRootNote());
                }
            }

            if (note.pitch[0] != -1)
            {
                for (int i = 0; i < note.pitch.Count; i++)
                {
                  

                    midi_player.GetComponent<MIDIPlayer>().NoteOn(MusicController.instance.RoundNote(note.pitch[i]) + MusicController.instance.GetRootNote());

                    Debug.Log(MusicController.instance.GetNoteName(note.pitch[i]) + ", " + MusicController.instance.GetNoteName(MusicController.instance.RoundNote(note.pitch[i])));
                    Debug.Log(note.pitch[i] + ", " + MusicController.instance.RoundNote(note.pitch[i]));
                }


            }

            last_note = note;

            melody_counter++;
            if(melody_counter >= melody.Count)
            {
                melody_counter = 0;
            }
            note = melody[melody_counter];

            nextTickMarkov += (60.0f / (MusicController.instance.bpm / note.length));


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
