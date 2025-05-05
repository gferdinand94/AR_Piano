using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class FallingNoteSystem : MonoBehaviour
{
    //private MidiFile midiFile = MidiFile.Read("C:\\Users\\gferd\\CS 595\\Project Prototyping\\test.mid");
    private MidiFile midiFile = MidiFile.Read(Application.dataPath + "\\midi\\tetris.mid");
    [System.Serializable]
    public class NoteEvent
    {
        public int midiNoteNumber; // MIDI note number (e.g. 60 = Middle C)
        //public MetricTimeSpan startTime;    // Time in seconds from the start
        public float startTime;    // Time in seconds from the start
        public float duration;     // Duration in seconds
        public float length;
        //public MetricTimeSpan duration;     // Duration in seconds
    }

    private HashSet<int> blackKeys;
    private HashSet<int> whiteKeys;

    public GameObject fallingNotePrefabWhite;
    public GameObject fallingNotePrefabBlack;
    public GameObject indicatorPrefabWhite;
    public Transform[] keyPositions; // Array of key positions (assign in Inspector)
    public List<NoteEvent> noteEvents = new List<NoteEvent>();
    public float noteSpeed = 0.5f; // Fall speed in meters/second
    //public Transform pianoRoot;
    public static Transform pianoRoot;

    private float songStartTime;

    void Start()
    {
        pianoRoot = GameObject.FindWithTag("PianoRoot").transform;

        Console.WriteLine(Application.dataPath);
        blackKeys = new HashSet<int>() 
        { 
            1, 
            4, 6, 
            9, 11, 13, 
            16, 18, 
            21, 23, 25, 
            28, 30, 
            33, 35, 37,
            40, 42,
            45, 47, 49,
            52, 54,
            57, 59, 61,
            64, 66,
            69, 71, 73,
            76, 78,
            81, 83, 85,
        };

        whiteKeys = new HashSet<int>()
        {
            0,  2,  3,  5,  7,
            8,  10, 12, 14, 15,
            17, 19, 20, 22, 24,
            26, 27, 29, 31, 32,
            34, 36, 38, 39, 41,
            43, 44, 46, 48, 50,
            51, 53, 55, 56, 58,
            60, 62, 63, 65, 67,
            68, 70, 72, 74, 75,
            77, 79, 80, 82, 84,
            86, 87
        };

        // Getting notes, tempo, and duration from MIDI file.
        TempoMap tempoMap = midiFile.GetTempoMap();
        IEnumerable<Note> notes = midiFile.GetNotes();
        int corrector = 21;

        foreach (var note in notes)
        {
            var startTimeMetric = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalMicroseconds / 1_000_000.0;
            float startTime = (float)startTimeMetric;

            var durationMetric = LengthConverter.ConvertTo<MetricTimeSpan>(note.Length, note.Time, tempoMap).TotalMicroseconds / 1_000_000.0;
            float duration = (float)durationMetric;

            float length = (float)durationMetric;

            //noteEvents.Add(new NoteEvent { midiNoteNumber = note.NoteNumber - corrector, startTime = startTime * 5.0f, duration = duration * 10.0f });
            noteEvents.Add(new NoteEvent { midiNoteNumber = note.NoteNumber - corrector, startTime = startTime * 3.0f, duration = duration * 10.0f, length = length });

        }

        //// Add some test notes manually
        //for (int i = 0; i < 88; i++)
        //{
        //    noteEvents.Add(new NoteEvent { midiNoteNumber = i, startTime = (float)i, duration = 10f });
        //}
        //noteEvents.Add(new NoteEvent { midiNoteNumber = 0, startTime = 1f, duration = 10f });
        //noteEvents.Add(new NoteEvent { midiNoteNumber = 2, startTime = 2f, duration = 10f });
        //noteEvents.Add(new NoteEvent { midiNoteNumber = 4, startTime = 3f, duration = 10f });

        songStartTime = Time.time;
        StartCoroutine(SpawnNotesCoroutine());
    }

    IEnumerator SpawnNotesCoroutine()
    {
        foreach (NoteEvent note in noteEvents)
        {
            float delay = note.startTime - (Time.time - songStartTime);
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            SpawnNote(note);
        }
    }

    void SpawnNote(NoteEvent note)
    {
        if (note.midiNoteNumber < 0 || note.midiNoteNumber >= keyPositions.Length)
        {
            Debug.LogWarning("Note out of key range!");
            return;
        }

        Transform key = keyPositions[note.midiNoteNumber];
        //Vector3 spawnPos = key.position + new Vector3(0, 5.0f, 0); // Spawn above key

        Vector3 spawnPos = key.position + key.up * 5.0f;
        //Vector3 spawnPos = key.position - key.forward * 5.0f;
        Vector3 spawnIndicator = key.position + new Vector3(0, 0.1f, 0.4f);
        //Vector3 spawnIndicator = key.position + (key.up * 0.1f) + (key.forward * 0.4f);
        Quaternion indicatorRotate = Quaternion.Euler(90f, 0, 0);

        //GameObject fallingNote;

        if (whiteKeys.Contains(note.midiNoteNumber))
        {
            GameObject fallingNote = Instantiate(fallingNotePrefabWhite, spawnPos, Quaternion.identity, pianoRoot);
            fallingNote.AddComponent<FallingNoteMover>().Init(note.duration, noteSpeed);

            GameObject indicator = Instantiate(indicatorPrefabWhite, spawnIndicator, Quaternion.Euler(90f, 0, 0), pianoRoot);
            indicator.AddComponent<IndicatorNote>().Init(fallingNote.transform);

            fallingNote.transform.rotation = Quaternion.Inverse(pianoRoot.rotation);

        }
        else
        {
            GameObject fallingNote = Instantiate(fallingNotePrefabBlack, spawnPos, Quaternion.identity, pianoRoot);
            fallingNote.AddComponent<FallingNoteMover>().Init(note.duration, noteSpeed);
            fallingNote.transform.rotation = Quaternion.Inverse(pianoRoot.rotation);

        }


        //GameObject fallingNote = Instantiate(fallingNotePrefabWhite, spawnPos, Quaternion.identity, pianoRoot);
        //fallingNote.AddComponent<FallingNoteMover>().Init(note.duration, noteSpeed);
    }

    void SpawnNote(int midiNote)
    {
        Transform key = keyPositions[midiNote];
        if (key == null) return;

        Vector3 spawnPosition = key.position + new Vector3(0, 0.5f, 0); // 0.5m above key
        GameObject note = Instantiate(fallingNotePrefabWhite, spawnPosition, Quaternion.identity, pianoRoot);
        note.name = $"FallingNote_{midiNote}";
    }

    // Internal class to move falling note
    public class FallingNoteMover : MonoBehaviour
    {
        private float lifetime;
        private float speed;

        private float lengthY;

        public void Init(float duration, float fallSpeed)
        {
            lifetime = duration;
            speed = fallSpeed; // * 0.1f;

            lengthY = duration * 0.05f;

        }

        void Update()
        {
            Vector3 currentScale = transform.localScale;
            currentScale.y = lengthY;
            transform.localScale = currentScale;
            transform.rotation = pianoRoot.rotation;

            //transform.position += Vector3.down * speed * Time.deltaTime;
            transform.position += pianoRoot.up * -speed * Time.deltaTime;
            //transform.position += pianoRoot.forward * speed * Time.deltaTime;

            // Get length of half of note
            float noteTopEdge = transform.localPosition.y + (lengthY / 2.0f);
            //float noteTopEdge = transform.localPosition.z + (lengthY / 2.0f);

            if (noteTopEdge < -0.01f)
                //if (transform.localPosition.z > 0.01f)
                Destroy(gameObject);
                //Destroy(gameObject);
            lifetime -= Time.deltaTime;
            //if (lifetime <= 0)
            //    Destroy(gameObject);
        }
    }

    public class IndicatorNote : MonoBehaviour
    {
        private Transform indicator;
        public void Init(Transform note)
        {
            indicator = note;
            gameObject.GetComponent<Renderer>().enabled = false;
        }
        void Update()
        {
            transform.rotation = pianoRoot.rotation * Quaternion.Euler(90f, 0, 0);
            if (indicator.localPosition.y < 0.25f)
                //if (indicator.localPosition.z > -0.25f)
                gameObject.GetComponent<Renderer>().enabled = true;

            if (indicator.localPosition.y < 0.0f)
                //if (indicator.localPosition.z > 0.0f)
                Destroy(gameObject);
        }
    }

    void SpawnIndicator(Transform note, Vector3 pos)
    {
        if (note.localPosition.y < 0.25f)
        {
            GameObject indicator = Instantiate(indicatorPrefabWhite, pos, Quaternion.Euler(90f, 0, 0), pianoRoot);
            indicator.AddComponent<IndicatorNote>().Init(note);
        }
    }
}