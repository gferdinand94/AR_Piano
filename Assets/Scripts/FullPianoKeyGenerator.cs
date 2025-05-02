using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FullPianoKeyGenerator : MonoBehaviour
{
    [Header("Key Dimensions (in meters)")]
    public float whiteKeyWidth = 0.023f;
    public float whiteKeyDepth = 0.15f;
    public float whiteKeyHeight = 0.02f;

    public float blackKeyWidth = 0.014f;
    public float blackKeyDepth = 0.1f;
    public float blackKeyHeight = 0.03f;

    [Header("Keyboard Settings")]
    public int numberOfWhiteKeys = 52;
    public Material whiteKeyMaterial;
    public Material blackKeyMaterial;

    private readonly int[] blackKeyPattern = { 1, 1, 0, 1, 1, 1, 0 }; // Pattern over 7 white keys

    //public float offset = whiteKeyWidth / 16.0f;

    void Start()
    {
        GenerateKeyboard();
    }

    void GenerateKeyboard()
    {
        float whiteKeyOffset = whiteKeyWidth + (whiteKeyWidth / 16.0f);
        float blackKeyOffset = whiteKeyOffset / whiteKeyWidth;
        int octaveIndex = 0;
        int whiteKeyIndex = 0;
        int midiNote = 21; // Start at A0 (MIDI note 21) if doing full 88-key layout

        for (int i = 0; i < numberOfWhiteKeys; i++)
        {
            // ----- Generate White Key -----
            GameObject whiteKey = GameObject.CreatePrimitive(PrimitiveType.Cube);
            whiteKey.name = $"WhiteKey_{i}_MIDI_{midiNote}";
            whiteKey.transform.parent = transform;
            whiteKey.transform.localScale = new Vector3(whiteKeyWidth, whiteKeyHeight, whiteKeyDepth);

            whiteKey.transform.localPosition = new Vector3(i * -whiteKeyOffset , whiteKeyHeight / 2f, 0); // 0.0235f

            //Material whiteMaterial = whiteKey.GetComponent<Renderer>().material;

            //whiteMaterial.color = Color.white;

            if (whiteKeyMaterial != null)
                whiteKey.GetComponent<Renderer>().material = whiteKeyMaterial;

            // ----- Check if this position has a black key -----
            int patternPos = octaveIndex % 7;
            if (i < 3) continue;
            else if (blackKeyPattern[patternPos] == 1 && i < numberOfWhiteKeys - 1) // Avoid overshooting
            {
                // Black key sits between this and the next white key
                GameObject blackKey = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blackKey.name = $"BlackKey_{i}_MIDI_{midiNote + 1}";
                blackKey.transform.parent = transform;
                blackKey.transform.localScale = new Vector3(blackKeyWidth, blackKeyHeight, blackKeyDepth);

                // Centered between this and next white key
                float blackX = (i * - blackKeyOffset) * whiteKeyWidth + (whiteKeyWidth * 0.5f);
                float blackY = blackKeyHeight / 2f;
                float blackZ = -(whiteKeyDepth - blackKeyDepth) / 2f;
                blackKey.transform.localPosition = new Vector3(blackX, blackY, blackZ);

                //Material blackMaterial = blackKey.GetComponent<Renderer>().material;
                //blackMaterial.color = Color.black;

                if (blackKeyMaterial != null)
                    blackKey.GetComponent<Renderer>().material = blackKeyMaterial;
            }

            // Update MIDI note (white keys only)
            midiNote += 1;

            // Step through pattern
            octaveIndex++;
            if (octaveIndex == 7)
                octaveIndex = 0;
        }
    }
}