using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

[RequireComponent(typeof(AudioSource))]
public class MIDIPlayer : MonoBehaviour
{
    public enum Instrument
    {
        Piano1,
        Piano2,
        Piano3,
        Honkytonk,
        ElectricPiano1,
        ElectricPiano2,
        Harpsichord,
        Clav,
        Celesta,
        Glockenspiel,
        MusicBox,
        Vibraphone,
        Marimba,
        Xylophone,
        TubularBell,
        Santur,
        Organ1,
        Organ2,
        Organ3,
        ChurchOrgan1,
        ReedOrgan,
        Accordion,
        Harmonica,
        Bandoneon,
        NylonGtr,
        SteelGtr,
        JazzGtr,
        CleanGtr,
        MuteGtr,
        OverdriveGtr,
        DistortionGtr,
        HarmonicGtr,
        AcousticBass,
        FingerBass,
        PickedBass,
        FretlessBass,
        SlapBass1,
        SlapBass2,
        SynthBass1,
        SynthBass2,
        Violin,
        Viola,
        Cello,
        Contrabass,
        TremoloStr,
        PizzicatoStr,
        Harp,
        Timpani,
        Strings,
        SlowStrings,
        SynthStrings1,
        SynthStrings2,
        ChoirAahs,
        VoiceOohs,
        SynthVox,
        OrchestraHit,
        Trumpet,
        Trombone,
        Tuba,
        MuteTrumpet,
        FrenchHorns,
        Brass1,
        SynthBrass1,
        SynthBrass2,
        SopranoSax,
        AltoSax,
        TenorSax,
        BaritoneSax,
        Oboe,
        EnglishHorn,
        Bassoon,
        Clarinet,
        Piccolo,
        Flute,
        Recorder,
        PanFlute,
        BottleBlow,
        Shakuhachi,
        Whistle,
        Ocarina,
        SquareWave,
        SawWave,
        SynthCalliope,
        ChifferLead,
        Charang,
        SoloVox,
        FifthSawWave,
        BassLead,
        Fantasia,
        WarmPad,
        Polysynth,
        SpaceVoice,
        BowedGlass,
        MetalPad,
        HaloPad,
        SweepPad,
        IceRain,
        Soundtrack,
        Crystal,
        Atmosphere,
        Brightness,
        Goblin,
        EchoDrops,
        StarTheme,
        Sitar,
        Banjo,
        Shamisen,
        Koto,
        Kalimba,
        Bagpipe,
        Fiddle,
        Shanai,
        TinkleBell,
        Agogo,
        SteelDrums,
        Woodblock,
        Taiko,
        MeloTom1,
        SynthDrum,
        ReverseCym,
        GtFretNoise,
        BreathNoise,
        Seashore,
        Bird,
        Telephone1,
        Helicopter,
        Applause,
        Gunshot,
        Standard,
        Room,
        Power,
        Electronic,
        TREigthZeroEight,
        Jazz,
        Brush,
        Orchestra,
        SFX
    }
    
    //Public
    //Check the Midi's file folder for different songs
    public string midiFilePath = "Midis/Groove.mid";
    public bool ShouldPlayFile = true;

    //Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;
    public int midiNoteVolume = 100;
    //[Range(0, 127)] //From Piano to Gunshot
    public Instrument instrument = Instrument.SynthBass1;
    
    public int midiInstrument;
    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    private MidiSequencer midiSequencer;
    private  StreamSynthesizer midiStreamSynthesizer;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 127.0f;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
        midiInstrument = (int)instrument;

        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        
        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        //midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    void LoadSong(string midiPath)
    {
        midiSequencer.LoadMidi(midiPath, false);
        midiSequencer.Play();
    }

    // Start is called just before any of the
    // Update methods is called the first time.
    void Start()
    {
    }

    // Update is called every frame, if the
    // MonoBehaviour is enabled.
    void Update()
    {
        if (!midiSequencer.isPlaying)
        {
            //if (!GetComponent<AudioSource>().isPlaying)
            if (ShouldPlayFile)
            {
                LoadSong(midiFilePath);
            }
        }
        else if (!ShouldPlayFile)
        {
            midiSequencer.Stop(true);
        }

        
    }

    public void NoteOn(int pitch)
    {

        midiStreamSynthesizer.NoteOn(0, pitch, midiNoteVolume, midiInstrument);
    }

    public void NoteOff(int pitch)
    {
        midiStreamSynthesizer.NoteOff(0, pitch);
    }

    private void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("lol");
        if (collision.gameObject.tag == "NoteBlock")
        {
            //Debug.Log("note, " + collision.gameObject.GetComponent<SoundBlock>().note);
            midiStreamSynthesizer.NoteOn(0, collision.gameObject.GetComponent<SoundBlock>().note, midiNoteVolume, midiInstrument);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "NoteBlock")
        {
            midiStreamSynthesizer.NoteOff(0, collision.gameObject.GetComponent<SoundBlock>().note);
        }
    }

    // See http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.OnAudioFilterRead.html for reference code
    //	If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
    //
    //	The filter is inserted in the same order as the MonoBehaviour script is shown in the inspector. 	
    //	OnAudioFilterRead is called everytime a chunk of audio is routed thru the filter (this happens frequently, every ~20ms depending on the samplerate and platform). 
    //	The audio data is an array of floats ranging from [-1.0f;1.0f] and contains audio from the previous filter in the chain or the AudioClip on the AudioSource. 
    //	If this is the first filter in the chain and a clip isn't attached to the audio source this filter will be 'played'. 
    //	That way you can use the filter as the audio clip, procedurally generating audio.
    //
    //	If OnAudioFilterRead is implemented a VU meter will show up in the inspector showing the outgoing samples level. 
    //	The process time of the filter is also measured and the spent milliseconds will show up next to the VU Meter 
    //	(it turns red if the filter is taking up too much time, so the mixer will starv audio data). 
    //	Also note, that OnAudioFilterRead is called on a different thread from the main thread (namely the audio thread) 
    //	so calling into many Unity functions from this function is not allowed ( a warning will show up ). 	
    private void OnAudioFilterRead(float[] data, int channels)
    {
        //This uses the Unity specific float method we added to get the buffer
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }

    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        Debug.Log("NoteOn: " + note.ToString() + " Velocity: " + velocity.ToString());
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        Debug.Log("NoteOff: " + note.ToString());
    }
}
