using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ScaleType { MAJOR, MINOR, DORIAN, PHRYGIAN, LYDIAN, MIXOLYDIAN, LOCRIAN, PENTATONIC };
public enum ScaleNote { C, C_Sharp, D, D_Sharp, E, F, F_Sharp, G, G_Sharp, A, A_Sharp, B, };


public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;

    public GameObject[] musicObjects;

    
    public float note_spawn_rate;
    public float rest_spawn_rate;

    [Range(50.0f, 300.0f)]
    public float bpm;

    public ScaleNote scale_note;
    public ScaleType scale_type;

    public int time_sig_top = 4;
    public int time_sig_bottom = 4;

    public string midi_file_name;

    public float shortest_note_length = 0.25f;

    public float time_step;

    bool done = false;
    bool sent = false;

    List<int>[] chords;

    // Return the notes of the current scale
    // In semitones
    public int[] GetScale()
    {
        switch (scale_type)
        {
            case ScaleType.MAJOR:
                return new int[7] { 0, 2, 4, 5, 7, 9, 11 };
            case ScaleType.MINOR:                        
                return new int[7] { 0, 2, 3, 5, 7, 8, 10 };
            case ScaleType.DORIAN:                       
                return new int[7] { 0, 2, 3, 5, 7, 9, 10 };
            case ScaleType.PHRYGIAN:                     
                return new int[7] { 0, 1, 3, 5, 7, 8, 10 };
            case ScaleType.LYDIAN:                       
                return new int[7] { 0, 2, 4, 6, 7, 9, 11 };
            case ScaleType.MIXOLYDIAN:                   
                return new int[7] { 0, 2, 4, 5, 7, 9, 10 };
            case ScaleType.LOCRIAN:                      
                return new int[7] { 0, 1, 3, 5, 6, 8, 10 };
            case ScaleType.PENTATONIC:                   
                return new int[7] { 0, 2, 4, 0, 7, 9, 12 };
        }

        return null;
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
                return i;
            }
        }

        // Returns first chord
        return 0;
    }

    public int GetNoteInChord(int pitch, int note_in_chord)
    {
        int[] scale = GetScale();

        int num = GetChordNumber(pitch);
        int base_pitch = GetBasicPitch(pitch);
        int base_note = GetRootNote();

        int final_note = 0;

        if (note_in_chord == 0)
        {
            final_note = scale[num];
        }
        else if (note_in_chord == 1)
        {
            if (num + 2 >= 7)
            {
                final_note = scale[num - 5];
            }
            else
            {
                final_note = scale[num + 2];
            }
        }
        else if (note_in_chord == 2)
        {
            if (num + 4 >= 7)
            {
                final_note = scale[num - 3];
            }
            else
            {
                final_note = scale[num + 4];
            }
        }

        return final_note + base_note;
    }

    //public string [] GetNotes(int note_count)
    //{
    //    string[] beats = new string[note_count];

    //    int[] current_scale = GetScale();

    //    for(int i = 0; i < note_count; i++)
    //    {
    //        // If true make a note
    //        if(Random.Range(0.0f, 1.0f) >= rest_spawn_rate)
    //        {
    //            beats[i] = notes[current_scale[(int)Random.Range(0, 7)]];
    //        }
    //        else
    //        {
    //            beats[i] = "";
    //        }
    //    }

    //    return beats;
    //}

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
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

    
    IEnumerator SetUp()
    {     
        Debug.Log("2");

        yield return new WaitForSeconds(1.0f);

        Debug.Log("1");

        yield return new WaitForSeconds(1.0f);

        double start_tick = AudioSettings.dspTime + (60.0f / (bpm / 2.0f));

        foreach (GameObject go in musicObjects)
        {
            go.SendMessage("Init", start_tick);
        }

        Debug.Log("Go");

        yield return null;
    }

    // Update is called once; per frame
    void Update ()
    {
        time_step = 60.0f/(bpm / shortest_note_length);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            bpm += Time.deltaTime * 10.0f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            bpm -= Time.deltaTime * 10.0f;
        }
    }
}
