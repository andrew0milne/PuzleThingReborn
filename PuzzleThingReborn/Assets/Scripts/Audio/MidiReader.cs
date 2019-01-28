using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NAudio.Midi;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

public struct MidiHolder
{
    public int pitch;
    public long bar;
    public float beat;
    public float length;
    public float time;

    public void Init(int p, long br, float bt, float l, float t)
    {
        pitch = p;
        bar = br;
        beat = bt;
        length = l;
        time = t;
    }

    public void Print()
    {
        Debug.Log("Pitch: " + pitch + ", Bar: " + bar + ", Beat: " + beat + ", Length: " + length + ", Time: " + time);
    }
}

public class MidiReader : MonoBehaviour
{

    public string midi_file_name;
    MidiFile midi_file;

    public List <MidiHolder>[] midi_holder;
    int number_of_channels;
    const int max_channels = 16;

    public int channel = 0;

    public GameObject cube;

    public GameObject[] parents;

    public float max_time = 0.0f;

    public float speed = 0.0f;

    

    public bool on = true;

    // Use this for initialization
    void Start ()
    {
        //Debug.Log(Application.dataPath + "/Audio/Midi Files/" + midi_file_name + ".mid");

        midi_holder = new List<MidiHolder>[max_channels];

        for(int i = 0; i < max_channels; i++)
        {
            midi_holder[i] = new List<MidiHolder>();
        }


        

        ReadInMidi();

        
        
        for (int i = 0; i < max_channels; i++)
        {
            foreach(MidiHolder mh in midi_holder[i])
            {
                GameObject temp = Instantiate(cube, new Vector3(((mh.bar * 4) + (mh.beat))* 2.0f, mh.pitch / 10.0f, i * 2.0f), Quaternion.identity, parents[i].transform);

                //temp.GetComponent<SoundBlock>().frequency = ;
                temp.GetComponent<SoundBlock>().Init(mh.pitch);
                temp.transform.localScale = new Vector3(mh.length * 2.0f, 0.5f, 1.0f);
                temp.transform.position += new Vector3(mh.length / 4.0f, 0.0f, 0.0f);

                Color colour = Color.white;

                switch(i)
                {
                    case 0:
                        colour = Color.red;
                        break;
                    case 1:
                        colour = Color.blue;
                        break;
                    case 2:
                        colour = Color.green;
                        break;
                    case 3:
                        colour = Color.yellow;
                        break;
                    case 4:
                        colour = Color.magenta;
                        break;
                    case 5:
                        colour = Color.cyan;
                        break;
                    case 6:
                        colour = Color.white;
                        break;
                    case 7:
                        colour = Color.black;
                        break;
                    case 8:
                        colour = new Color(1.0f, 0.5f, 0.0f);
                        break;
                    case 9:
                        colour = new Color(0.0f, 0.5f, 1.0f);
                        break;
                    case 10:
                        colour = new Color(1.0f, 0.0f, 0.5f);
                        break;
                    case 11:
                        colour = new Color(1.0f, 0.5f, 1.0f);
                        break;
                    case 12:
                        colour = new Color(1.0f, 1.0f, 0.5f);
                        break;
                    case 13:
                        colour = new Color(1.0f, 1.0f, 0.5f);
                        break;
                    case 14:
                        colour = new Color(1.0f, 0.5f, 0.0f);
                        break;
                    case 15:
                        colour = new Color(1.0f, 0.5f, 0.0f);
                        break;

                }
 
                
                 temp.GetComponent<Renderer>().material.color = colour;
                

                //mh.Print();
            }
        }
        
    }

    void ReadInMidi()
    {
        midi_file = new MidiFile(Application.dataPath + "/Audio/Midi Files/" + midi_file_name + ".mid");

        MidiEventCollection midi_events = midi_file.Events;

        for (int n = 0; n < midi_file.Tracks; n++)
        {
            foreach (var midiEvent in midi_file.Events[n])
            {
                if (MidiEvent.IsNoteOn(midiEvent))
                {
                    ToMBT((NoteEvent)midiEvent, midiEvent.AbsoluteTime, midi_file.DeltaTicksPerQuarterNote, null);
                }
            }
        }

        speed = midi_file.DeltaTicksPerQuarterNote;

        Camera.main.gameObject.SendMessage("Activate");

        //midi_file.Events.
    }

    private void ToMBT(NoteEvent note, long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
    {
        int beatsPerBar;
        int ticksPerBar;
        int ticksPerBeat;
        long bar;
        float beat;
        long tick;
        long time;

        string s = note.ToString();

        string len = "Len: ";

        if (s.Contains(len))
        {
            int num = s.IndexOf(len) + 5;

            //print(s.Substring(num));
            beatsPerBar = timeSignature == null ? 4 : timeSignature.Numerator;
            ticksPerBar = timeSignature == null ? ticksPerQuarterNote * 4 : (timeSignature.Numerator * ticksPerQuarterNote * 4) / (1 << timeSignature.Denominator);
            ticksPerBeat = ticksPerBar / beatsPerBar;
            bar = 1 + (eventTime / ticksPerBar);
            beat = 1 + ((eventTime % ticksPerBar) / ticksPerBeat);
            tick = eventTime % ticksPerBeat;
            time = eventTime / ticksPerBeat;

            float temp_length = (float)(int.Parse(s.Substring(num)) / (float)ticksPerQuarterNote);

            MidiHolder temp_midi = new MidiHolder();

            temp_length = Mathf.Round(temp_length * 4) / 4;
            if(temp_length < 0.25f)
            {
                temp_length = 0.25f;
            }

            temp_midi.Init(note.NoteNumber, bar, beat, temp_length, time);
            temp_midi.Print();

            midi_holder[note.Channel - 1].Add(temp_midi);

            if (note.Channel > number_of_channels)
            {
                number_of_channels = note.Channel;
            }

            if(eventTime + temp_length > max_time)
            {
                max_time = eventTime + temp_length;
            }
        }

    }

    // Cleans up the list of midi events
    // - Combines notes that occur at the same time (chords)
    // - Adds an event for sections with no notes (rests)
    void CleanUp(int channel)
    {
        List<MidiHolder> new_midi = new List<MidiHolder>();

        int iter = 0;

        // Iterates through midi events
        foreach(MidiHolder mh in midi_holder[channel])
        {
            // Doesn't include last event as that last note is either a part of the chords that came before it, or a single note
            if (iter < midi_holder[channel].Count - 2)
            {
                // If the note happens at the same time as the next one
                if (mh.bar == midi_holder[channel][iter++].bar && mh.beat == midi_holder[channel][iter++].beat)
                {

                }
            }

            iter++;
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
