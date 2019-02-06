using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ScaleType { MAJOR, MINOR, DORIAN, PHRYGIAN, LYDIAN, MIXOLYDIAN, LOCRIAN, PENTATONIC };
public enum ScaleNote { A, A_Sharp, B, C, C_Sharp, D, D_Sharp, E, F, F_Sharp, G, G_Sharp };


public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;

    public GameObject[] musicObjects;

    
    public float note_spawn_rate;
    public float rest_spawn_rate;

    private string[] notes = new string[13] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "AA" };

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

    public string [] GetNotes(int note_count)
    {
        string[] beats = new string[note_count];

        int[] current_scale = GetScale();

        for(int i = 0; i < note_count; i++)
        {
            // If true make a note
            if(Random.Range(0.0f, 1.0f) >= rest_spawn_rate)
            {
                beats[i] = notes[current_scale[(int)Random.Range(0, 7)]];
            }
            else
            {
                beats[i] = "";
            }
        }

        return beats;
    }

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
        chords[0].Add(4);
        chords[0].Add(5);

        chords[1].Add(4);
        chords[1].Add(2);

        chords[2].Add(5);
        chords[2].Add(3);

        chords[3].Add(1);
        chords[3].Add(4);

        chords[4].Add(2);
        chords[4].Add(5);
        chords[4].Add(0);

        chords[5].Add(3);
        chords[5].Add(1);
        
       
    }

    public int GetNextChord(int chords_name)
    {
        Debug.Log(chords_name);

        int num = Random.Range(0, chords[chords_name].Count);

        return chords[chords_name][num];
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

    // Update is called once per frame
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
