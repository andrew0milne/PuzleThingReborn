using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NAudio.Midi;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

public class MidiHolder : MonoBehaviour
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

        name = "" + pitch + time;
    }

    public void Print()
    {
        Debug.Log("Pitch: " + pitch + ", Length: " + length + ", Time: " + time);
    }
}

public class NextNote: MonoBehaviour
{
    public MidiHolder note;
    public int freq;

    public void Print()
    {
        note.Print();
        Debug.Log(freq);
    }
}

public class DependHolder : MonoBehaviour
{
    public MidiHolder note;
    public List<NextNote> next_note;
    public int max_freq;

    public void Init(MidiHolder a)
    {
        note = a;
        next_note = new List<NextNote>();
        max_freq = 0;
    }

    
    public void AddNote(MidiHolder new_note)
    {
        bool found = false;
        

        // Checks if the combination has already occured
        foreach (NextNote nn in next_note)
        {
            if (new_note.pitch == nn.note.pitch && new_note.length == nn.note.length)
            {
                // The combination has occured
                nn.freq++;
                found = true;
                break;
            }
        }

        // A new combination
        if (!found)
        {
            NextNote temp_note = new NextNote();
            temp_note.note = new_note;
            temp_note.freq = 1;

            next_note.Add(temp_note);
        }
    }

    public void SumFreq()
    {
        foreach(NextNote nn in next_note)
        {
            max_freq += nn.freq;
        }
    }

    public void Print()
    {
        foreach(NextNote nn in next_note)
        {
            note.Print();
            nn.Print();
        }
        Debug.Log("----------");
    }
}

public class MidiReader : MonoBehaviour
{

    public string midi_file_name;
    MidiFile midi_file;

    public List<MidiHolder>[] midi_holder;
    int number_of_channels;
    const int max_channels = 16;

    public int channel = 0;

    public GameObject cube;

    public GameObject[] parents;

    public float max_time = 0.0f;

    public float speed = 0.0f;

    List<MidiHolder> new_midi;// = new List<MidiHolder>();

    public bool on = true;

    public int notes_to_make;

    public bool generate_visualiser = false;

    // Use this for initialization
    void Awake()
    {
        //Debug.Log(Application.dataPath + "/Audio/Midi Files/" + midi_file_name + ".mid");

        midi_holder = new List<MidiHolder>[max_channels];

        for (int i = 0; i < max_channels; i++)
        {
            midi_holder[i] = new List<MidiHolder>();
        }

        List<MidiHolder> new_midi = new List<MidiHolder>();

        if (generate_visualiser)
        {
            //ReadInMidi();

            //CleanUp(0);

            
        }

    }

    public void ReadInMidi()
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

        
        for (int i = 0; i < 16; i++)
        {
            CleanUp(i);

            if (generate_visualiser)
            {
                foreach (MidiHolder mh in midi_holder[i])
                {
                    //mh.Print();

                    GameObject temp = Instantiate(cube, new Vector3(mh.time, mh.pitch / 10.0f, 0.0f), Quaternion.identity, parents[i].transform);

                    //temp.GetComponent<SoundBlock>().Init(mh.pitch);
                    temp.transform.localScale = new Vector3(mh.length, 0.5f, 1.0f);
                    temp.transform.position += new Vector3(mh.length / 2.0f, 0.0f, i * 1.0f);

                    Color colour = Color.blue;

                    if (mh.pitch == -1)
                    {
                        //temp.transform.localScale = new Vector3(mh.length * 2.0f, 5.0f, 1.0f);
                        temp.transform.position = new Vector3(temp.transform.position.x, 6.0f, temp.transform.position.z + 2.0f);
                        colour = Color.black;
                        //Destroy(temp.GetComponent<Rigidbody>());
                    }

                    temp.GetComponent<Renderer>().material.color = colour;
                }
            }
        }

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

            if (eventTime + temp_length > max_time)
            {
                max_time = eventTime + temp_length;
            }
        }
    }

    // Cleans up the list of midi events
    // - Combines notes that occur at the same time (chords)
    // - Adds an event for sections with no notes (rests)
    public void CleanUp(int channel)
    {
        //new_midi = new List<MidiHolder>();

        // Adds last event to new midi holder
        // new_midi.Add(midi_holder[channel][0]);

        // Iterates through midi events
        for (int i = 0; i < midi_holder[channel].Count - 1; i++)
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


            if (current.time + current.length != next.time)
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

    bool CheckNotes(MidiHolder mh1, MidiHolder mh2)
    {
        if (mh1.pitch == mh2.pitch && mh1.length == mh2.length)
        {
            return true;
        }

        return false;
    }

    public MidiHolder GetFirstNote()
    {
        //for(int i = 0; i < midi_holder[channel].Count - 1; i++)
        //{
        //    if(midi_holder[channel][i].pitch != -1)
        //    {
        //        return midi_holder[channel][i];
        //    }
        //}

        return midi_holder[channel][0];
    }

    public List<DependHolder> FreqDistribution()
    {
        List<DependHolder> freq_distribution = new List<DependHolder>();

        // Creates the frequency distribution
        for (int i = 0; i < midi_holder[channel].Count - 2; i++)
        {
            bool found = false;

            // Checks to see if the current note sequence has been found before
            foreach (DependHolder dh in freq_distribution)
            {
                if (CheckNotes(midi_holder[channel][i], dh.note))
                {
                    found = true;
                    dh.AddNote(midi_holder[channel][i + 1]);
                    break;
                }
            }

            // If note create a new note sequence and add it the the distribution list
            if (!found)
            {
                DependHolder temp_holder = new DependHolder();
                temp_holder.Init(midi_holder[channel][i]);

                temp_holder.AddNote(midi_holder[channel][i + 1]);

                freq_distribution.Add(temp_holder);
            }
        }


        foreach (DependHolder dh in freq_distribution)
        {
            dh.SumFreq();
        }

        return freq_distribution;  
    }

    public MidiHolder GetNote(List<DependHolder> freq, MidiHolder previous_note)
    {
        foreach (DependHolder dh in freq)
        {
            //markov_midi[markov_midi.Count - 1].Print();
            //dh.note.Print();

            if (CheckNotes(previous_note, dh.note))
            {
                int random = Random.Range(0, dh.max_freq);
                int p = 0;

                //Debug.Log(dh.max_freq);

                foreach (NextNote nn in dh.next_note)
                {
                    p += nn.freq;


                    if (p >= random)
                    {
                        MidiHolder new_note = new MidiHolder();
                        new_note.pitch = nn.note.pitch;
                        new_note.length = nn.note.length;

                        return new_note;                     
                    }

                }

                break;
            }
        }

        return GetFirstNote();
    }

    List<MidiHolder> Markov()
    {
        List<MidiHolder> markov_midi = new List<MidiHolder>();
        List<DependHolder> freq_distribution = new List<DependHolder>();

        freq_distribution = FreqDistribution();

        markov_midi.Add(midi_holder[0][0]);

        for (int i = 0; i < notes_to_make; i++)
        {
            MidiHolder new_note = GetNote(freq_distribution, markov_midi[markov_midi.Count - 1]);

            if(new_note != null)
            {
                markov_midi.Add(new_note);
            }
                        
            //foreach (DependHolder dh in freq_distribution)
            //{
            //    //markov_midi[markov_midi.Count - 1].Print();
            //    //dh.note.Print();

            //    if (CheckNotes(markov_midi[markov_midi.Count - 1], dh.note))
            //    {
            //        int random = Random.Range(0, dh.max_freq);
            //        int p = 0;

            //        //Debug.Log(dh.max_freq);

            //        foreach (NextNote nn in dh.next_note)
            //        {
            //            p += nn.freq;


            //            if (p >= random)
            //            {
            //                MidiHolder new_note = new MidiHolder();
            //                new_note.pitch = nn.note.pitch;
            //                new_note.length = nn.note.length;

            //                markov_midi.Add(new_note);
            //                break;
            //            }

            //        }

            //        break;
            //    }
            //}

            //Debug.Log(i);
        }

        float time = 0.0f;

        for (int i = 0; i < markov_midi.Count - 1; i++)
        {
            markov_midi[i].time = time;
            time += markov_midi[i].length;
        }

        return markov_midi;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
