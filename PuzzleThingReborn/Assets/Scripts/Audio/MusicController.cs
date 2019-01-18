using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Scale { MAJOR, MINOR, DORIAN, PHRYGIAN, LYDIAN, MIXOLYDIAN, LOCRIAN, PENTATONIC };

public class MusicController : MonoBehaviour
{
    public GameObject[] musicObjects;

    public float bpm;

    public float note_spawn_rate;
    public float rest_spawn_rate;

    private string[] notes = new string[13] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "AA" };

    public Scale scale;
    

    public int[] GetScale()
    {
        switch (scale)
        {
            case Scale.MAJOR:
                return new int[8] { 0, 2, 4, 5, 7, 9, 11, 12 };
            case Scale.MINOR:
                return new int[8] { 0, 2, 3, 5, 7, 8, 10, 12 };
            case Scale.DORIAN:
                return new int[8] { 0, 2, 3, 5, 7, 9, 10, 12 };
            case Scale.PHRYGIAN:
                return new int[8] { 0, 1, 3, 5, 7, 8, 10, 12 };
            case Scale.LYDIAN:
                return new int[8] { 0, 2, 4, 6, 7, 9, 11, 12 };
            case Scale.MIXOLYDIAN:
                return new int[8] { 0, 2, 4, 5, 7, 9, 10, 12 };
            case Scale.LOCRIAN:
                return new int[8] { 0, 1, 3, 5, 6, 8, 10, 12 };
            case Scale.PENTATONIC:
                return new int[8] { 0, 2, 4, 0, 7, 9, 12, 12 };
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

	// Use this for initialization
	void Start ()
    {
        musicObjects = GameObject.FindGameObjectsWithTag("MusicObject");

        foreach(GameObject go in musicObjects)
        {
            go.SendMessage("InitBPM", bpm);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
