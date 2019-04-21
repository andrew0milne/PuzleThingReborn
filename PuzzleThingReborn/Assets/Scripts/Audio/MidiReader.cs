using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NAudio.Midi;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

public class MidiHolder : ScriptableObject
{
    public List<int> pitch;
    public long bar;
    public float beat;
    public float length;
    public float og_length;
    public float time;

    public void Init(int p, long br, float bt, float l, float t)
    {
        pitch = new List<int>();
        pitch.Add(p);
        bar = br;
        beat = bt;
        length = l;
        og_length = l;
        time = t;

        //name = "" + pitch + time;
    }

    public void Init(int p, float l)
    {
        pitch = new List<int>();
        pitch.Add(p);
        length = l;
    }

    public void Print()
    {
        Debug.Log("Pitch: " + pitch + ", Length: " + length + ", Time: " + time);
    }
}

public class NextNote: ScriptableObject
{
    public MidiHolder note;
    public int freq;

    public void Print()
    {
        //note.Print();
        //Debug.Log(freq);
    }
} 

public class DependHolder : ScriptableObject
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
            if (CheckNotes(new_note, nn.note))
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
            NextNote temp_note = ScriptableObject.CreateInstance <NextNote>();
            temp_note.note = new_note;
            temp_note.freq = 1;

            next_note.Add(temp_note);
        }
    }

    bool CheckNotes(MidiHolder mh1, MidiHolder mh2)
    {       
        if (mh1.pitch.Count == mh2.pitch.Count && mh1.og_length == mh2.og_length)
        {
            for (int i = 0; i < mh1.pitch.Count; i++)
            {
                if (mh1.pitch[i] != mh2.pitch[i])
                {
                    return false;
                }
            }

            return true;
        }

        return false;      
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

public class Theme: ScriptableObject
{
    public List<MidiHolder>[] theme;

    void Init()
    {

    }
}

[System.Serializable]
public struct MidiFileInput
{
    public string name;
    public int channel;
    public ScaleNote scale_note;
    public ScaleType scale_type;
}

public class MidiReader : MonoBehaviour
{

    public MidiFileInput[] midi_files;
    public ScaleNote og_scale_note;
    public ScaleType og_scale_type;

    int scale_root;

    MidiFile midi_file;

    public List<MidiHolder>[] midi_holder;
    int number_of_channels;
    const int max_channels = 16;

    public float min_rest_length = 0.25f;

    public GameObject cube;

    public GameObject[] parents;

    public float max_time = 0.0f;

    public float speed = 0.0f;

    List<MidiHolder> new_midi;// = new List<MidiHolder>();

    public bool on = true;

    public int notes_to_make;

    public bool generate_visualiser = false;

    MidiHolder first_note;

    // Use this for initialization
    void Start()
    {
        //Debug.Log(Application.dataPath + "/Audio/Midi Files/" + midi_file_name + ".mid");

        //MusicController.instance.scale_note = og_scale_note;
        //MusicController.instance.scale_type = og_scale_type;

        print(MusicController.instance);

        MusicController.instance.UpdateScales(og_scale_type, og_scale_note);

        scale_root = MusicController.instance.GetRootNote();

        midi_holder = new List<MidiHolder>[midi_files.Length];

        for (int i = 0; i < midi_files.Length; i++)
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
        for (int j = 0; j < midi_files.Length; j++)
        {
            // Gets the MIDI file
            midi_file = new MidiFile(Application.dataPath + "/Audio/Midi Files/" + midi_files[j].name + ".mid");

            MidiEventCollection midi_events = midi_file.Events;

            Debug.Log(midi_file);

            // Gets all the midi notes
            
            foreach (var midiEvent in midi_events[midi_files[j].channel])
            {            
                if (MidiEvent.IsNoteOn(midiEvent))
                {
                    ConvertEvent((NoteEvent)midiEvent, midiEvent.AbsoluteTime, midi_file.DeltaTicksPerQuarterNote, null, j);
                }
            }
        
            //for (int i = 0; i < midi_file_name.Length; i++)
            //{
            CleanUp(j);         

                //if (generate_visualiser)
                //{
                //    foreach (MidiHolder mh in midi_holder[i])
                //    {
                //        //mh.Print();
                //
                //        for (int p = 0; p < mh.pitch.Count; p++)
                //        {
                //            GameObject temp = Instantiate(cube, new Vector3(mh.time, mh.pitch[p] / 10.0f, 0.0f), Quaternion.identity, parents[i].transform);
                //
                //            //temp.GetComponent<SoundBlock>().Init(mh.pitch);
                //            temp.transform.localScale = new Vector3(mh.length, 0.5f, 1.0f);
                //            temp.transform.position += new Vector3(mh.length / 2.0f, 0.0f, i * 1.0f);
                //
                //            Color colour = Color.blue;
                //
                //            if (mh.pitch[0] == -1)
                //            {
                //                //temp.transform.localScale = new Vector3(mh.length * 2.0f, 5.0f, 1.0f);
                //                temp.transform.position = new Vector3(temp.transform.position.x, 6.0f, temp.transform.position.z + 0.1f);
                //                colour = Color.black;
                //                //Destroy(temp.GetComponent<Rigidbody>());
                //            }
                //
                //            temp.GetComponent<Renderer>().material.color = colour;
                //        }
                //    }
                //}
            //}

            foreach (MidiHolder mh in midi_holder[j])
            {

                if (mh.pitch[0] != -1)
                {
                    first_note = mh;
                    break;
                }
            }
        }
    }

    
    public float RoundToDecimal(float number, float deciml, bool can_be_zero)
    {
        float d = 1 / deciml;
        float num = Mathf.Round(number * d) / d;
        if (num < deciml && !can_be_zero)
        {
            num = deciml;
        }

        return num;
    }

    private void ConvertEvent(NoteEvent note, long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature, int midi_file)
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

            MidiHolder temp_midi = ScriptableObject.CreateInstance<MidiHolder>();// new MidiHolder();

            // Round to nearest 0.25
            temp_length = RoundToDecimal(temp_length, MusicController.instance.shortest_note_length, false);

            time = RoundToDecimal(time, MusicController.instance.shortest_note_length, true);

            temp_midi.Init(note.NoteNumber - scale_root, bar, beat, temp_length, time);
            //temp_midi.Print();

            midi_holder[midi_file].Add(temp_midi);

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

        int rest_count1 = 0;
        int rest_count2 = 0;

        // Iterates through midi events
        for (int i = 0; i < midi_holder[channel].Count - 1; i++)
        {
            // Next note in midi_holder
            MidiHolder current = midi_holder[channel][i];
            MidiHolder next = midi_holder[channel][i + 1];

            if(current.length == 0.25f && current.pitch[0] == -1)
            {
                rest_count1++;
            }
            else if (current.length == 0.5f && current.pitch[0] == -1)
            {
                rest_count2++;
            }

            // makes a rest
            if (current.time + current.length != next.time && current.time != next.time)
            {
                float new_length = next.time - (current.time + current.length);

                if (new_length > min_rest_length)
                { 
                    float new_time = current.time + current.length;


                    MidiHolder rest = ScriptableObject.CreateInstance<MidiHolder>();
                    rest.Init(-1, -1, -1, new_length, new_time);

                    midi_holder[channel].Insert(i + 1, rest);
                    i--;
                }
                else
                {
                    midi_holder[channel][i].length += new_length;
                }
            }
        }

        print("0.25 rest count = " + rest_count1);
        print("0.5 rest count = " + rest_count2);

        List<MidiHolder>[] temp_midi_holder = new List<MidiHolder>[max_channels];
        for (int i = 0; i < max_channels; i++)
        {
            temp_midi_holder[i] = new List<MidiHolder>();
        }

        for (int i = 0; i < midi_holder[channel].Count - 1; i++)
        {
            // Next note in midi_holder
            MidiHolder current = midi_holder[channel][i];
            MidiHolder next = midi_holder[channel][i + 1];

            MidiHolder temp = ScriptableObject.CreateInstance<MidiHolder>();
            temp.Init(current.pitch[0], current.bar, current.beat, current.length, current.time);

            float time = next.time;

            // If chord
            while (temp.time == time)
            {
                temp.pitch.Add(next.pitch[0]);

                if (i + 1 < midi_holder[channel].Count)
                {
                    i++;
                    if (i + 1 < midi_holder[channel].Count)
                    {
                      next = midi_holder[channel][i + 1];
                      time = next.time;
                    }
                }
                else
                {
                    time = -1;
                }

            }

            temp_midi_holder[channel].Add(temp);
        }

        

        midi_holder[channel] = temp_midi_holder[channel];

    }

    bool CheckNotes(MidiHolder mh1, MidiHolder mh2, bool check_length, bool debug)
    {
        if (check_length)
        {
            if (debug)
            { print("                                    " + mh1.pitch.Count + "=" + mh2.pitch.Count + " , " + mh1.length + "=" + mh2.length); }
            if (mh1.pitch.Count == mh2.pitch.Count && mh1.length == mh2.length)
            {
                for (int i = 0; i < mh1.pitch.Count; i++)
                {
                    if (debug)
                    { print("                                    " + mh1.pitch[i] + "=" + mh2.pitch[i]); }
                    
                    if (mh1.pitch[i] != mh2.pitch[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
        else
        {
            if (mh1.pitch.Count == mh2.pitch.Count)
            {
                for (int i = 0; i < mh1.pitch.Count; i++)
                {
                    if (mh1.pitch[i] != mh2.pitch[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }

    public MidiHolder GetFirstNote(int song_number)
    {

        Debug.Log("midi_holder " + midi_holder[song_number].Count);
        Debug.Log("Song number " + song_number);

        return midi_holder[song_number][0];


    }

    public List<DependHolder> FreqDistribution(int song_number)
    {
        List<DependHolder>freq_distribution = new List<DependHolder>();

        // Creates the frequency distribution
        for (int i = 0; i < midi_holder[song_number].Count - 2; i++)
        {
            bool found = false;

            // Checks to see if the current note sequence has been found before
            foreach (DependHolder dh in freq_distribution)
            {
                if (CheckNotes(midi_holder[song_number][i], dh.note, true, false))
                {
                    found = true;
                    dh.AddNote(midi_holder[song_number][i + 1]);
                    break;
                }
            }

            // If note create a new note sequence and add it the the distribution list
            if (!found)
            {
                DependHolder temp_holder = ScriptableObject.CreateInstance<DependHolder>();
                temp_holder.Init(midi_holder[song_number][i]);

                temp_holder.AddNote(midi_holder[song_number][i + 1]);

                freq_distribution.Add(temp_holder);
            }
        }


        foreach (DependHolder dh in freq_distribution)
        {
            dh.SumFreq();
        }

        return freq_distribution;  
    }

    //public MidiHolder GetNoteWithChord(List<DependHolder> freq, MidiHolder previous_note, int chord_root)
    //{
    //    foreach (DependHolder dh in freq)
    //    {
    //        if (CheckNotes(previous_note, dh.note, true))
    //        {
    //            foreach(NextNote nn in dh.next_note)
    //            {
    //                if(MusicController.instance.IsInChord(nn.note.pitch[0], chord_root))
    //                {
    //                    MidiHolder new_note = ScriptableObject.CreateInstance<MidiHolder>();
    //                    new_note.pitch = nn.note.pitch;
    //                    new_note.length = nn.note.length;

    //                    return new_note;
    //                }
    //            }

    //            break;
    //        }
    //    }

    //    Debug.Log("ERROR: NO NEXT NOTE FOUND, CHECKING ALL NOTES");

    //    foreach (DependHolder dh in freq)
    //    {
    //        if(MusicController.instance.IsInChord(dh.note.pitch[0], chord_root))
    //        {
    //            MidiHolder new_note = ScriptableObject.CreateInstance <MidiHolder>();
    //            new_note.pitch = dh.note.pitch;
    //            new_note.length = dh.note.length;

    //            return new_note;
    //        }
    //    }

    //    Debug.Log("ERROR: NO NOTE GENERATED, GENERATING BASE NOTE");

    //    MidiHolder note = ScriptableObject.CreateInstance <MidiHolder>();
    //    note.pitch = new List<int>();
    //    int[] scale = MusicController.instance.GetScale();
    //    note.pitch.Add(scale[chord_root]);
    //    note.length = 1.0f;

    //    return note;
    //}

    public MidiHolder GetNote(List<DependHolder> freq, MidiHolder previous_note, int song_number)
    {
        //Debug.Log("Get next note");

        //print("--- " + Time.time + " ---");
       // print(freq.Count);

        bool check_length = true;

        for (int i = 0; i < 2; i++)
        {
            foreach (DependHolder dh in freq)
            {

                //print(dh.note.pitch[0] + ", " + dh.note.length + ", " + dh.note.pitch.Count);

                if (CheckNotes(previous_note, dh.note, check_length, false))
                {                

                    float random = Random.Range(0, (float)dh.max_freq);
                    int p = 0;

                    //print(dh.next_note.Count);
                    foreach (NextNote nn in dh.next_note)
                    {
                        p += nn.freq;                    

                        //Debug.Log(p + " >= " + random + "?");

                        if (p >= random)
                        {
                            //Debug.Log("yes");

                            MidiHolder new_note = ScriptableObject.CreateInstance<MidiHolder>();
                            new_note.pitch = nn.note.pitch;
                            new_note.length = nn.note.length;

                            //print(dh.note.pitch[0] + ", " + dh.note.length + " --> " + new_note.pitch[0] + ", " + new_note.length);

                            return new_note;
                        }
                        //Debug.Log("no");
                    }

                    break;
                }
            }



            Debug.Log("WARNING: NOTE " + previous_note.pitch[0] + ", " + previous_note.length + ", " + previous_note.pitch.Count);
            print("-----------------------------------------------------------------------------------------------------------");

            if (check_length)
            {
                Debug.Log("WARNING: NO NOTE GENERATED, RETRYING WITHOUT NOTE LENGTH");
            }

            check_length = false;
        }
        Debug.Log("WARNING: NO NOTE GENERATED, CREATING A RANDOM NOTE");

        MidiHolder note = ScriptableObject.CreateInstance <MidiHolder>();

        note = freq[Random.Range(0, freq.Count)].note;

        //note = GetFirstNote(song_number);
        //note.length = MusicController.instance.shortest_note_length;

        return note;
    }

    List<MidiHolder> Markov(int song_number)
    {
        List<MidiHolder> markov_midi = new List<MidiHolder>();
        List<DependHolder> freq_distribution = new List<DependHolder>();

        freq_distribution = FreqDistribution(song_number);

        markov_midi.Add(midi_holder[0][0]);

        for (int i = 0; i < notes_to_make; i++)
        {
            MidiHolder new_note = GetNote(freq_distribution, markov_midi[markov_midi.Count - 1], song_number);

            if(new_note != null)
            {
                markov_midi.Add(new_note);
            }
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
