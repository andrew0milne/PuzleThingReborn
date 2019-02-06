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

    // Use this for initialization
    void Start ()
    {
        reader_script = midi_reader.GetComponent<MidiReader>();
        reader_script.ReadInMidi();

        chord = new int[3];

        
    }

    void Init(double b)
    {
        bpm = MusicController.instance.bpm;
        shortest_note_length = MusicController.instance.shortest_note_length;

        //reader_script.CleanUp(0);
        freq_dist = reader_script.FreqDistribution();

        note = new MidiHolder();

        
        note = reader_script.GetFirstNote();
        
        last_note = note;
        last_note.pitch = -1;

        //double startTick = AudioSettings.dspTime;
        nextTick = b;

        playing = true;
    }
	
    int GetBasicPitch(int pitch)
    {
        int base_note = 33;
        int note = pitch;

        while(pitch > base_note + 11)
        {
            pitch -= 12;
        }

        return pitch;
    }

    // code
    void GetChord(int pitch, bool on)
    {
        if (on)
        {
            pitch += 33;

            int base_pitch = GetBasicPitch(pitch) + 12;

            int pitch_dif = base_pitch - 33;

            int[] scale_notes = MusicController.instance.GetScale();

            int num = 0;

            for(int i = 0; i < 7; i++)
            {
                if(pitch_dif == scale_notes[i])
                {
                    num = i;
                    break;
                }
            }

            chord[0] = base_pitch;
            if(num +2 >= 7)
            {
                chord[1] = base_pitch + scale_notes[num - 5];
            }
            else
            {
                chord[1] = base_pitch + scale_notes[num + 2];
            }
            
            if(num +5 >= 7)
            {
                chord[2] = base_pitch + scale_notes[num - 3];
            }
            else
            {
                chord[2] = base_pitch + scale_notes[num + 4];
            }

            midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[0]);
            midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[1]);
            midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[2]);
        }
        else
        {
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[0]);
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[1]);
            midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[2]);
        }
        
    }

	// Update is called once per frame
	void Update ()
    {
        if (playing)
        {
            if (AudioSettings.dspTime >= nextTick)
            {
                if(markov)
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
                else
                {
                    if (beat == 0.0f)// && note.pitch != -1)
                    {
                        midi_player.GetComponent<MIDIPlayer>().NoteOn(num_note + 33);
                    }
                    else if (beat == shortest_note_length * 4.0f)
                    {
                        midi_player.GetComponent<MIDIPlayer>().NoteOff(num_note + 33);
                        num_note = num_chord;
                    }

                    if (beat == shortest_note_length * 4.0f)// && note.pitch != -1)
                    {
                        //GetChord(note.pitch, true);
                        GetChord(num_chord, true);

                    }
                    else if (beat == 0.0f)
                    {
                        //GetChord(note.pitch, false);
                        GetChord(num_chord, false);
                        //num2 = Random.Range(0, 3);

                        num_chord = MusicController.instance.GetNextChord(num_chord);
                    }

                    nextTick += MusicController.instance.time_step;
                }

                

                

                beat += shortest_note_length;
                if(beat >= 2.0f)
                {
                    beat = 0.0f;
                    bar++;
                }

                //nextTick += MusicController.instance.time_step;// (60.0f / (MusicController.instance.bpm / note.length));
            }
        }
    }
}
