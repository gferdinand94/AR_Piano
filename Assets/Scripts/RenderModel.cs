using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderModel : MonoBehaviour
{

    public GameObject pianoRoot;

    public void SetPianoVisible(bool visible)
    {
        MeshRenderer[] renderers = pianoRoot.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    public void OnTogglePiano()
    {
        bool isVisible = pianoRoot.GetComponentInChildren<MeshRenderer>().enabled;
        SetPianoVisible(!isVisible);
    }
}
