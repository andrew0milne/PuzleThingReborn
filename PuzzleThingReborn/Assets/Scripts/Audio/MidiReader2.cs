using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NAudio.Midi;

public class MidiReader2 : MonoBehaviour
{

    public string midi_file_name;

    MidiFile midi_file;

    public List<MidiHolder> midi_holder;

    

    // Use this for initialization
    void Start ()
    {
        ReadIn();

    }

    void ReadIn()
    {
        midi_holder = new List<MidiHolder>();

        midi_file = new MidiFile(Application.dataPath + "/Audio/Midi Files/" + midi_file_name + ".mid");

        MidiEventCollection midi_events = midi_file.Events;

        

        // Gets all the midi notes

        for (int i = 0; i < midi_file.Tracks; i++)
        {
            Debug.Log(midi_events.GetTrackEvents(i));

            //if(midi_events.)

            //Debug.Log(i);

        }

        for (int i = 0; i < 16; i++)
        {

            foreach (var midi in midi_events[i])
            {
                if (MidiEvent.IsNoteOn(midi))
                {
                    ConvertEvent((NoteEvent)midi, midi.AbsoluteTime, midi_file.DeltaTicksPerQuarterNote, null);
                }
            }
        }
    }

    private void ConvertEvent(NoteEvent note, long eventTime, int ticksPerQuarterNote, TimeSignatureEvent timeSignature)
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
            //temp_length = RoundToDecimal(temp_length, MusicController.instance.shortest_note_length, false);

            //time = RoundToDecimal(time, MusicController.instance.shortest_note_length, true);

            temp_midi.Init(note.NoteNumber, bar, beat, temp_length, time);
            //temp_midi.Print();

            midi_holder.Add(temp_midi);

            
        }
    }


}
