using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
//using Melanchall.DryWetMidi.MusicTheory;

public class ReadMidi : MonoBehaviour
{
    private MidiFile midiFile = MidiFile.Read("C:\\Users\\gferd\\CS 595\\Project Prototyping\\test.mid");
    //TempoMap tempoMap = midiFile.GetTempoMap();
    //IEnumerable<Note> notes = midiFile.GetNotes();

    // Start is called before the first frame update
    void Start()
    {
        TempoMap tempoMap = midiFile.GetTempoMap();
        IEnumerable<Note> notes = midiFile.GetNotes();
        int corrector = 21;
        //TempoMap tempoMap = midiFile.GetTempoMap();
        //IEnumerable<Note> notes = midiFile.GetNotes();

        foreach (var note in notes) 
        {
            //var startTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var startTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalMicroseconds / 1_000_000.0;
            var duration = LengthConverter.ConvertTo<MetricTimeSpan>(note.Length, note.Time, tempoMap);

            //Console.WriteLine($"Note: {note.NoteNumber - corrector}, Start: {startTime}, Duration: {duration}");

            //Debug.Log($"Note: {note.NoteNumber - corrector}, Start: {startTime}, Duration: {duration}");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
