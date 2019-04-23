using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ScaleType { LYDIAN, MAJOR, MIXOLYDIAN, DORIAN, MINOR, PHRYGIAN, LOCRIAN };

public enum ScaleNote { C, C_Sharp, D, D_Sharp, E, F, F_Sharp, G, G_Sharp, A, A_Sharp, B, };

public class Chord : ScriptableObject
{
    public int root;
    public float length;

    public void init(int r, float l)
    {
        root = r;
        length = l;
    }
}

public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;

    public GameObject[] musicObjects;


    [Header("Main Music Controllers")]
    [Tooltip("Controls variables such as the bpm, volume and rythmic density")]
    [Range(-1.0f, 1.0f)]
    public float intensity = 1.0f;
    [Tooltip("Controls variables such as the scale and root note (Basically how happy the music is)")]
    [Range(-1.0f, 1.0f)]
    public float valence = 0.0f;

    [Space]
    public float markov_intensity_lower_weighting = 0.7f;
    public float markov_intensity_upper_weighting = 0.7f;

    [Header("BPM")]
    [Range(50.0f, 300.0f)]
    public float bpm;
    public bool change_bpm = true;
    public float min_bpm = 80.0f;
    public float max_bpm = 150.0f;


    [Header("Scales")]
    public ScaleNote scale_note;
    public ScaleType scale_type;
    public int scale_type_num;

    public int time_sig_top = 4;
    public int time_sig_bottom = 4;

    public float shortest_note_length = 0.25f;

    public float time_step;

    bool done = false;
    bool sent = false;

    List<int>[] chords;

    public GameObject midi_reader;
    MidiReader reader_script;

    List<DependHolder> freq;

   

    public float min_dist = 1.0f;
    public float max_dist = 10.0f;

    //public GameObject player;
    //public GameObject enemy;

    public int song_number = 0;

    

    private void Awake()
    {
        print("hello");

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        musicObjects = GameObject.FindGameObjectsWithTag("MusicObject");

        StartCoroutine(SetUp());

        time_step = bpm / shortest_note_length;

        chords = new List<int>[6];

        for (int i = 0; i < 6; i++)
        {
            chords[i] = new List<int>();
        }


        chords[0].Add(1);
        chords[0].Add(2);
        chords[0].Add(3);
        chords[0].Add(3);
        chords[0].Add(4);
        chords[0].Add(4);
        chords[0].Add(5);

        chords[1].Add(4);
        chords[1].Add(2);

        chords[2].Add(5);
        chords[2].Add(3);

        chords[3].Add(1);
        chords[3].Add(4);
        chords[3].Add(4);
        chords[3].Add(4);

        chords[4].Add(2);
        chords[4].Add(5);
        chords[4].Add(0);
        chords[4].Add(0);

        chords[5].Add(3);
        chords[5].Add(1);
        chords[5].Add(1);

        chords[0].Add(4);
        chords[4].Add(5);
        chords[5].Add(3);
        chords[3].Add(0);

        reader_script = midi_reader.GetComponent<MidiReader>();
        reader_script.ReadInMidi();
        freq = reader_script.FreqDistribution(song_number);

        

        int[] scale = GetScale();

       

        //scale_type = (ScaleType)num;
    }

    

    IEnumerator SetUp()
    {
        Debug.Log("2");

        yield return new WaitForSeconds(1.0f);

        Debug.Log("1");

        yield return new WaitForSeconds(5.0f);

        double start_tick = AudioSettings.dspTime + (60.0f / (bpm / 2.0f));

        foreach (GameObject go in musicObjects)
        {
            go.SendMessage("Init", start_tick);
        }

        Debug.Log("Go");

        yield return null;
    }

    public int GetNumberOfSongs()
    {
        return midi_reader.GetComponent<MidiReader>().midi_files.Length;
    }

    public void UpdateScales(ScaleType st, ScaleNote sn)
    {
        scale_note = sn;
        scale_type = st;

        scale_type_num = (int)st;
    }

    // Return the notes of the current scale
    // In semitones
    public int[] GetScale()
    {
        int[] scale;
        
        scale_type = (ScaleType)scale_type_num;

        switch (scale_type)
        {
            case ScaleType.MAJOR:
                scale = new int[7] { 0, 2, 4, 5, 7, 9, 11 };
                break;
            case ScaleType.MINOR:
                scale = new int[7] { 0, 2, 3, 5, 7, 8, 10 };
                break;
            case ScaleType.DORIAN:
                scale = new int[7] { 0, 2, 3, 5, 7, 9, 10 };
                break;
            case ScaleType.PHRYGIAN:
                scale = new int[7] { 0, 1, 3, 5, 7, 8, 10 };
                break;
            case ScaleType.LYDIAN:
                scale = new int[7] { 0, 2, 4, 6, 7, 9, 11 };
                break;
            case ScaleType.MIXOLYDIAN:
                scale = new int[7] { 0, 2, 4, 5, 7, 9, 10 };
                break;
            case ScaleType.LOCRIAN:
                scale = new int[7] { 0, 1, 3, 5, 6, 8, 10 };
                break;
            //case ScaleType.PENTATONIC:
                //scale = new int[7] { 0, 2, 4, 0, 7, 9, 12 };
                //break;
            default:
                scale = new int[7] { 0, 2, 3, 5, 7, 8, 10 };
                break;
        }

        int root = GetRootNote();

        for(int i = 0; i < 7; i++)
        {
            scale[i] += root;
            scale[i] = GetBasicPitch(scale[i]);
        }

        return scale;
    }

    // Converts the pitch to be in the current scale   
    public int RoundNote(int pitch)
    {
        int[] scale = GetScale();

        int base_pitch = GetBasicPitch(pitch);
        int pitch_shift = PitchShiftDegree(pitch);

        foreach(int i in scale)
        {
            if(base_pitch == i)
            {
                return base_pitch + (12 * pitch_shift);
            }
            else if(base_pitch + 1 == i)
            {
                return base_pitch + 1 + (12 * pitch_shift);
            }
        }

        return pitch;
    }

    int GetWrappedIncrement(int note, int increment)
    {
        int[] scale = GetScale();
       
        if(note + increment >= 7)
        {
            return increment - 7;
        }

        return increment;
    }

    public bool IsInChord(int pitch, int chord_root)
    {
        int lowest_pitch = GetBasicPitch(pitch);
        int[] scale = GetScale();

        if (pitch == scale[chord_root])
        {
            return true;
        }
        else if (pitch == scale[GetWrappedIncrement(chord_root, 2)])
        {
            return true;
        }
        else if (pitch == scale[GetWrappedIncrement(chord_root, 4)])
        {
            return true;
        }

        return false;
    }

    // Returns the root note of the current scale
    // A = 0
    // A_Sharp = 1
    // So on
    public int GetRootNote()
    {
        switch(scale_note)
        {       
            case ScaleNote.C:
                return 0;
            case ScaleNote.C_Sharp:
                return 1;
            case ScaleNote.D:
                return 2;
            case ScaleNote.D_Sharp:
                return 3;
            case ScaleNote.E:
                return 4;
            case ScaleNote.F:
                return 5;
            case ScaleNote.F_Sharp:
                return 6;
            case ScaleNote.G:
                return 7;
            case ScaleNote.G_Sharp:
                return 8;
            case ScaleNote.A:
                return 9;
            case ScaleNote.A_Sharp:
                return 10;
            case ScaleNote.B:
                return 11;
            default:
                return 0;
        }
    }

    public string GetNoteName(int pitch)
    {
        int base_pitch = GetBasicPitch(pitch);

        switch(base_pitch)
        {
            case 0:
                return "C";
            case 1:
                return "C#";
            case 2:
                return "D";
            case 3:
                return "D#";
            case 4:
                return "E";
            case 5:
                return "F";
            case 6:
                return "F#";
            case 7:
                return "G";
            case 8:
                return "G#";
            case 9:
                return "A";
            case 10:
                return "A#";
            case 11:
                return "B";
            default:
                return "--";
        }
    }

    // Converts 'pitch' to the lowest version of that note
    // 48 (C4) -> 0
    public int GetBasicPitch(int pitch)
    {
        int note = pitch;
        
        while (note > 11)
        {
            note -= 12;
        }

        return note;
    }

    int PitchShiftDegree(int pitch)
    {
        int note = pitch;
        int pitch_shift = 0;

        while (note > 11)
        {
            note -= 12;
            pitch_shift++;
        }

        return pitch_shift;
    }


    int GetSemitoneDif(int note, int root)
    {
        if(root > note)
        {
            note += 12;
        }

        return note - root;
    }

    // Gets the chord number of note, based on the current scale
    // If in D Major, and the note is A
    // D -> 2, A -> 9
    // 9 - 2 = 5, chord = V
    public int GetChordNumber(int note)
    {
        int[] scale = GetScale();

        int lowest_pitch = GetSemitoneDif(GetBasicPitch(note), GetRootNote());

        for (int i = 0; i < 7; i++)
        {
            // Returns chord number
            if (lowest_pitch == scale[i])
            {
                Debug.Log(i);
                return i;
            }
        }

        // Returns first chord
        return 0;
    }

    public int GetNoteInChord(int root, int note_in_chord)
    {
        int[] scale = GetScale();

        //int num = GetChordNumber(pitch);
        //int base_pitch = GetBasicPitch(pitch);
        //int base_note = GetRootNote();

        int final_note = 0;

        if (note_in_chord == 0)
        {
            final_note = scale[root];
        }
        else if (note_in_chord == 1)
        {
            if (root + 2 >= 7)
            {
                final_note = scale[root - 5];
            }
            else
            {
                final_note = scale[root + 2];
            }
        }
        else if (note_in_chord == 2)
        {
            if (root + 4 >= 7)
            {
                final_note = scale[root - 3];
            }
            else
            {
                final_note = scale[root + 4];
            }
        }

        return final_note;// + base_note;
    }


    public int GetNextChord(int chords_name)
    {
        int num = Random.Range(0, chords[chords_name].Count);

        int[] scale = GetScale();

        return chords[chords_name][num];
    }

    public int GetChordRootNote(int chord_name)
    {
        int[] scale = GetScale();

        return scale[chord_name];
    }
  
    List<MidiHolder> GetBasicScale(int chords_per_phrase, float length_min, float length_max, float max_phrase_length)
    {
        List<MidiHolder> basic_scale = new List<MidiHolder>();
        MidiHolder first = ScriptableObject.CreateInstance <MidiHolder>();
        float len = Random.Range(length_min, length_max);
        len = RoundToDecimanl(len, shortest_note_length * 2.0f, false);


        first.Init(Random.Range(0, 6), len);

        float length_left = max_phrase_length - first.length;

        basic_scale.Add(first);

        for (int i = 1; i < chords_per_phrase - 1; i++)
        {

            MidiHolder temp = ScriptableObject.CreateInstance <MidiHolder>();
            float length = Random.Range(length_min, length_max);
            length = RoundToDecimanl(length, shortest_note_length * 2.0f, false);

            //Debug.Log(length_left + "-" + length + "=" + (length_left - length));

            if (length_left - length < 0)
            {
                length = length_left;
            }

            temp.Init(GetNextChord(basic_scale[basic_scale.Count - 1].pitch[0]), length);
            basic_scale.Add(temp);
            length_left -= temp.length;
            
        }

        if (length_left > 0)
        {
            MidiHolder last = ScriptableObject.CreateInstance <MidiHolder>();
            last.Init(GetNextChord(basic_scale[basic_scale.Count - 1].pitch[0]), length_left);
            basic_scale.Add(last);
        }
        
        return basic_scale;
    }

    List<MidiHolder> GetBasicMelody(float max_phrase_length, List<MidiHolder> chords, int song_number)
    {
        List<MidiHolder> melody = new List<MidiHolder>();
        float length_left = max_phrase_length;

        bool found_note = false;

        MidiHolder first = reader_script.GetFirstNote(song_number);

        for(int i = 0; i < 3; i++)
        {
            if (found_note == false)
            {
                foreach (DependHolder dh in freq)
                {
                    if (GetBasicPitch(dh.note.pitch[0]) == GetNoteInChord(GetBasicPitch(chords[0].pitch[0]), i))
                    {
                        Debug.Log("FOUND AT " + i);
                        first = dh.note;
                        found_note = true;
                        break;
                    }
                }
            }
        }

        melody.Add(first);
        length_left -= melody[0].length;

        while(length_left >= 0)
        {
            melody.Add(reader_script.GetNote(freq, melody[melody.Count - 1], song_number));
            length_left -= melody[melody.Count - 1].length;
        }

        foreach(MidiHolder mh in melody)
        {
            Debug.Log(GetNoteName(mh.pitch[0]));
        }

        return melody;
    }

    int[] GetFullChord(int root_note)
    {
        int[] full_chord = new int[4];

        full_chord[0] = GetNoteInChord(root_note, 0);
        full_chord[1] = GetNoteInChord(root_note, 1);
        full_chord[2] = GetNoteInChord(root_note, 2);
        full_chord[3] = full_chord[0] + 12;

        return full_chord;

    }

    public List<MidiHolder>[] GetTheme(bool scentence, int chords_per_phrase, float length_min, float length_max, float max_phrase_length, int song_number)
    {
        List<MidiHolder>[] scale_progression = new List<MidiHolder>[2];

        for(int i = 0; i < 2; i++)
        {
            scale_progression[i] = new List<MidiHolder>();
        }


        Debug.Log("Creating Chord sequence...");
        List<MidiHolder> basic_motif_chords = GetBasicScale(chords_per_phrase, length_min, length_max, max_phrase_length);
        List<MidiHolder> answer_chords = GetBasicScale(chords_per_phrase, length_min, length_max, max_phrase_length);
        List<MidiHolder> answer_2_chords = GetBasicScale(chords_per_phrase, length_min, length_max, max_phrase_length);
        Debug.Log("Finished Chord sequence...");

        answer_2_chords[answer_2_chords.Count - 1].pitch[0] = 0;

        Debug.Log("Creating Melodic sequence...");
        List<MidiHolder> basic_motif_melo = GetBasicMelody(max_phrase_length, basic_motif_chords, song_number);
        List<MidiHolder> answer_melo = GetBasicMelody(max_phrase_length, answer_chords, song_number);
        List<MidiHolder> answer_2_melo = GetBasicMelody(max_phrase_length, answer_2_chords, song_number);
        Debug.Log("Finished Melodic sequence...");

        //basic_motif_melo = AlterMelodyToChords(basic_motif_melo, basic_motif_chords);
        //answer_melo = AlterMelodyToChords(answer_melo, answer_chords);
        //answer_2_melo = AlterMelodyToChords(answer_2_melo, answer_2_chords);

        if (scentence)
        {
            // Add basic Motif
            foreach (MidiHolder c in basic_motif_chords)
            {
                scale_progression[0].Add(c);

            }

            
            // Add basic motif again
            foreach (MidiHolder c in basic_motif_chords)
            {
                scale_progression[0].Add(c);

            }

            // Add anser to basic motif
            foreach (MidiHolder c in answer_chords)
            {
                scale_progression[0].Add(c);

            }

            // Add end
            foreach (MidiHolder c in answer_2_chords)
            {
                scale_progression[0].Add(c);

            }


            // Add basic Motif
            foreach (MidiHolder c in basic_motif_melo)
            {
                scale_progression[1].Add(c);

            }
        
            // Add basic motif again
            foreach (MidiHolder c in basic_motif_melo)
            {
                scale_progression[1].Add(c);

            }

            // Add anser to basic motif
            foreach (MidiHolder c in answer_melo)
            {
                scale_progression[1].Add(c);

            }

            // Add end
            foreach (MidiHolder c in answer_2_melo)
            {
                scale_progression[1].Add(c);

            }
        }
        else
        {


            // Add basic Motif
            foreach (MidiHolder c in basic_motif_chords)
            {
                scale_progression[0].Add(c);

            }

            // Add anser to basic motif
            foreach (MidiHolder c in answer_chords)
            {
                scale_progression[0].Add(c);

            }

            // Add basic motif again
            foreach (MidiHolder c in basic_motif_chords)
            {
                scale_progression[0].Add(c);

            }

            // Add end
            foreach (MidiHolder c in answer_2_chords)
            {
                scale_progression[0].Add(c);

            }



            // Add basic Motif
            foreach (MidiHolder c in basic_motif_melo)
            {
                scale_progression[1].Add(c);

            }

            // Add anser to basic motif
            foreach (MidiHolder c in answer_melo)
            {
                scale_progression[1].Add(c);

            }

            // Add basic motif again
            foreach (MidiHolder c in basic_motif_melo)
            {
                scale_progression[1].Add(c);

            }

            // Add end
            foreach (MidiHolder c in answer_2_melo)
            {
                scale_progression[1].Add(c);

            }
        }
        return scale_progression;
    }


    // Update is called once; per frame
    public MidiHolder GetNote(MidiHolder note, int song_number)
    {
        return reader_script.GetNote(freq, note, song_number);
    }

    void UpdateVnI()
    {
        float num = (valence + 1) * 3;

        scale_type_num = 6 - (int)num;


        float intensity_lerp = (intensity + 1) / 2.0f;

        bpm = Mathf.Lerp(min_bpm, max_bpm, intensity_lerp);     
    }

    void Update ()
    {
        time_step = 60.0f/(bpm / shortest_note_length);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
                //bpm += Time.deltaTime * 10.0f;
             if(scale_type_num < 6)
             {
                scale_type_num++;
             }

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //bpm -= Time.deltaTime * 10.0f;
            if (scale_type_num > 0)
            {
                scale_type_num--;
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            bpm -= Time.deltaTime * 10.0f;   

        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            bpm += Time.deltaTime * 10.0f;         
        }

        if (change_bpm)
        {
            //UpdateBPM();
        }

        UpdateVnI();
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
}
