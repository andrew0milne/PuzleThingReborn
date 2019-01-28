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

    public string name;

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
        Debug.Log("Pitch: " + pitch + ", Length: " + length + ", Time: " + time);
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

    List<MidiHolder> new_midi;// = new List<MidiHolder>();

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

        List<MidiHolder> new_midi = new List<MidiHolder>();


        ReadInMidi();

        CleanUp(0);

        

        foreach (MidiHolder mh in midi_holder[0])
        {
            //mh.Print();

            GameObject temp = Instantiate(cube, new Vector3(mh.time, mh.pitch / 10.0f, 0.0f), Quaternion.identity);//, parents[i].transform);

            temp.GetComponent<SoundBlock>().Init(mh.pitch);
            temp.transform.localScale = new Vector3(mh.length, 0.5f, 1.0f);
            temp.transform.position += new Vector3(mh.length / 2.0f, 0.0f, 0.0f);

            Color colour = Color.blue;

            if (mh.pitch == -1)
            {
                //temp.transform.localScale = new Vector3(mh.length * 2.0f, 5.0f, 1.0f);
                temp.transform.position = new Vector3(temp.transform.position.x, 6.0f, temp.transform.position.z + 2.0f);
                colour = Color.black;
            }

            temp.GetComponent<Renderer>().material.color = colour;

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

    float RoundToDecimanl(float number, float deciml, bool can_be_zero)
    {
        float d = 1 / deciml;
        float num = Mathf.Round(number * d) / d;
        if (num < deciml && !can_be_zero)
        {
            num = deciml;
        }

        return num;
    }

    private void ToMBT(NoteEvent note, long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
    {
        int beatsPerBar;
        int ticksPerBar;
        int ticksPerBeat;
        long bar;
        float beat;
        long tick;
        float time;

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
            time = eventTime / (float)ticksPerBeat;

            float temp_length = (int.Parse(s.Substring(num)) / (float)ticksPerQuarterNote);

            MidiHolder temp_midi = new MidiHolder();

            // Round to nearest 0.25
            temp_length = RoundToDecimanl(temp_length, 0.25f, false);

            time = RoundToDecimanl(time, 0.25f, true);

            temp_midi.Init(note.NoteNumber, bar, beat, temp_length, time);
            //temp_midi.Print();

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
        //new_midi = new List<MidiHolder>();

        // Adds last event to new midi holder
        // new_midi.Add(midi_holder[channel][0]);

        // Iterates through midi events
        for (int i = 0 ; i < midi_holder[channel].Count - 1; i++)
        {
            
            // Next note in midi_holder
            MidiHolder current = midi_holder[channel][i];
            MidiHolder next = midi_holder[channel][i + 1];

            //// If chord
            //if (new_midi[new_midi.Count - 1].time == next.time)
            //{
            //    MidiHolder temp = new_midi[new_midi.Count - 1];
            //    temp.name += next.name + ":";
            //    new_midi.RemoveAt(new_midi.Count - 1);
            //    new_midi.Add(temp);
            //}
            // New note
            

            if(current.time + current.length != next.time)
            {
                MidiHolder rest = new MidiHolder();
                float new_length = next.time - (current.time + current.length);
                float new_time = current.time + current.length;
                rest.Init(-1, -1, -1, new_length, new_time);

                midi_holder[channel].Insert(i + 1, rest);
                i--;
            }
        }
    }

    List<MidiHolder> Markov()
    {
        List<MidiHolder> markov_midi = new List<MidiHolder>();

        int lowest_pitch = 10000000;
        int highest_pitch = 0;

        foreach(MidiHolder mh in midi_holder[0])
        {
            if (mh.pitch != -1)
            {
                if (mh.pitch < lowest_pitch)
                {
                    lowest_pitch = mh.pitch;
                }

                if(mh.pitch > highest_pitch)
                {
                    highest_pitch = mh.pitch;
                }
            }
        }

        int pitch_range = highest_pitch - lowest_pitch;

        int[,] freq_dist = new int[pitch_range + 1, pitch_range + 1];

        for (int i = 0; i < midi_holder[0].Count - 2; i++)
        {
            int current_pitch
            freq_dist[midi_holder[0][i].pitch, midi_holder[0][i + 1].pitch]++;
        }

        return markov_midi;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
