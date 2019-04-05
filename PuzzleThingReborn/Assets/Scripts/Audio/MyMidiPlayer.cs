using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MyMidiPlayer : MonoBehaviour
{

    private double nextTick = 0.0f;
    //private double nextTickChords = 0.0f;

    public GameObject midi_reader;
    MidiReader reader_script; 

    MidiHolder note;
    MidiHolder next_note;
    MidiHolder last_note;
    //int[] chord;

    public GameObject midi_player;
    MIDIPlayer midi_play;

    public float bpm = 140.0f;
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

    public int markov_offset = 0;
    public int chord_offset = 0;

    float markov_beat_counter = 0.0f;
    float chord_beat_counter = 0.0f;

    List<Theme> themes;

 
    int[] theme_counter;

    int chord_num = 0;

    //List<MidiHolder> melody;
    //int melody_counter = 0;

    public bool g = false;

    // Use this for initialization
    void Start()
    {
        //reader_script = midi_reader.GetComponent<MidiReader>();


        //chord = new int[3];
    }

    void Init(double b)
    {
        bpm = MusicController.instance.bpm;
        shortest_note_length = MusicController.instance.shortest_note_length;

        //reader_script.CleanUp(0);
        

        //double startTick = AudioSettings.dspTime;
        nextTick = b;
        //nextTickChords = b;

        playing = true;

        Debug.Log("hello there");

        themes = new List<Theme>();


        Debug.Log("midi count " + MusicController.instance.GetNumberOfSongs());

        for(int i = 0; i < MusicController.instance.GetNumberOfSongs(); i++)
        {
            Theme temp_theme = ScriptableObject.CreateInstance<Theme>();

            temp_theme.theme = MusicController.instance.GetTheme(true, 4, 1.0f, 2.0f, 4.0f, i);

            Debug.Log("Theme " + i + ": " + temp_theme.theme.Length);

            themes.Add(temp_theme);
        }


        //List<MidiHolder>[] temp_1;
        //List<MidiHolder>[] temp_2;

        //temp_1 = MusicController.instance.GetTheme(true, 4, 1.0f, 2.0f, 4.0f, 0);
        //temp_2 = MusicController.instance.GetTheme(true, 4, 1.0f, 2.0f, 4.0f, 1);
        //melody = MusicController.instance.GetThemeNotes(true, 4.0f);

        

        theme_counter = new int[2] { 0, 0 };

        Debug.Log(themes.Count);

        note = themes[MusicController.instance.song_number].theme[1][0];



        last_note = note;
        last_note.pitch[0] = -1;
    }



    // code
    //void GetChord(int pitch, bool on)
    //{
    //    if (on)
    //    {
    //        for(int i = 0; i <3; i++)
    //        {
    //            chord[i] = MusicController.instance.GetNoteInChord(pitch, i);
    //            chord[i] += 48;
    //            midi_player.GetComponent<MIDIPlayer>().NoteOn(chord[i] + (12* chord_offset));


    //        }
    //        //Debug.Log(MusicController.instance.GetNoteName(chord[0]) + ", " + MusicController.instance.GetNoteName(chord[1]) + ", " + MusicController.instance.GetNoteName(chord[2]));
    //    }
    //    else
    //    {
    //        midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[0] + (12 * chord_offset));
    //        midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[1] + (12 * chord_offset));
    //        midi_player.GetComponent<MIDIPlayer>().NoteOff(chord[2] + (12 * chord_offset));
    //    }

    //}

    //void GetChord(MidiHolder note)
    //{
    //    MidiHolder[] chord = new MidiHolder[3];

    //    for (int i = 0; i < 3; i++)
    //    {
    //        chord[i] = ScriptableObject.CreateInstance <MidiHolder>();
    //        chord[i].pitch = new List<int>();
    //        chord[i].pitch.Add(0);
    //        chord[i].pitch[0] = MusicController.instance.GetNoteInChord(note.pitch[0], i);
    //        chord[i].pitch[0] += 48;
    //        chord[i].pitch[0] += (12 * chord_offset);
    //        chord[i].length = note.length;
    //    }

    //    for (int i = 0; i < 3; i++)
    //    {
    //        StartCoroutine(PlayNote(chord[i]));
    //    }
    //}


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

    IEnumerator PlayNote(MidiHolder note, MidiHolder next_note, int song_num)
    {
         if (note.pitch[0] != -1)
         {

            MidiHolder temp_note = note;

            float length = (note.length);
            float new_len = 0.0f;

            float new_pitch = 0.0f;

            MidiHolder new_note = ScriptableObject.CreateInstance<MidiHolder>();

            bool make_new_note = Random.Range(0.0f, 2.0f) < MusicController.instance.intensity;

            if (make_new_note)
            {
                //Debug.Log("new note");

                float old_len = length;
                length /= 2.0f;

                length = RoundToDecimanl(length, MusicController.instance.shortest_note_length, false);

                new_len = old_len - length;

                new_note = MusicController.instance.GetNote(note, song_num);
            }

            


            for (int i = 0; i < note.pitch.Count; i++)
            {
                temp_note.pitch[i] = MusicController.instance.RoundNote(note.pitch[i]) + (12 * markov_offset);
            }

            for (int i = 0; i < note.pitch.Count; i++)
            {
                midi_player.GetComponent<MIDIPlayer>().NoteOn(temp_note.pitch[i]);
            }

            yield return new WaitForSeconds(length * (60.0f / MusicController.instance.bpm));// (note.length - 0.1f) * (60.0f/MusicController.instance.bpm));

            for (int i = 0; i < note.pitch.Count; i++)
            {
                midi_player.GetComponent<MIDIPlayer>().NoteOff(temp_note.pitch[i]);
            }

            if (make_new_note)
            {

                new_pitch = MusicController.instance.RoundNote(new_note.pitch[0]) + (12 * markov_offset);

                midi_player.GetComponent<MIDIPlayer>().NoteOn((int)new_pitch);


                yield return new WaitForSeconds(new_len * (60.0f / MusicController.instance.bpm));// (note.length - 0.1f) * (60.0f/MusicController.instance.bpm));


                midi_player.GetComponent<MIDIPlayer>().NoteOff((int)new_pitch);

            }

        }

        yield return null;
    }

    int GetNextIter(int current, int max)
    {
        //Debug.Log(current + ", " + max);

        int num = current + 1;

        if (num >= max)
        {
            num = 0;
        }

        //Debug.Log(num);

        return num;
    }

    void UpdateBeat()
    {
        beat += MusicController.instance.shortest_note_length;

        if(beat >= 4.0f)
        {
            beat = 0.0f;
            bar++;
        }
    }

    void PlayScale()
    {
        if (AudioSettings.dspTime >= nextTick) //&& chords)
        {

            /*///if (AudioSettings.dspTime >= nextTickChords && chords)
            if (chord_beat_counter >= theme[0][chord_num].length && chords)
            {
                chord_num = theme_counter[0] - 1;
                if (chord_num < 0)
                {
                    chord_num = theme[0].Count - 1;
                }

                GetChord(theme[0][chord_num]);

                chord_beat_counter = 0.0f;

                //nextTickChords += (60.0f / (MusicController.instance.bpm / theme[0][theme_counter[0]].length));

                //



                theme_counter[0]++;
                if (theme_counter[0] >= theme[0].Count)
                {
                    theme_counter[0] = 0;
                }
            }

            // 
            /chord_beat_counter += MusicController.instance.shortest_note_length;*/




            if (markov_beat_counter >= themes[MusicController.instance.song_number].theme[1][theme_counter[1]].length && markov)
            {
                markov_beat_counter = 0.0f;

                next_note = themes[MusicController.instance.song_number].theme[1][GetNextIter(theme_counter[1], themes[MusicController.instance.song_number].theme[1].Count)];

                StartCoroutine(PlayNote(note, next_note, MusicController.instance.song_number));

                theme_counter[1] = GetNextIter(theme_counter[1], themes[MusicController.instance.song_number].theme[1].Count);

                //theme_counter[1]++;
                //if (theme_counter[1] >= theme[1].Count)
                //{
                //    theme_counter[1] = 0;
                //}
                note = themes[MusicController.instance.song_number].theme[1][theme_counter[1]];


            }

            markov_beat_counter += MusicController.instance.shortest_note_length;

            nextTick += MusicController.instance.time_step;

            UpdateBeat();
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            PlayScale();
        }
    }
}
