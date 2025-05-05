using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignPiano : MonoBehaviour
{

    public GameObject pianoRoot;

    public Slider xPos, yPos, zPos;
    public Slider rot;      // rotation about x axis
    public Slider scale;

    // Setting the values of the buffers for the min and max values for position, rotation, and scale of the model within the sliders.
    // These values seem to be the ideal values, but can be adjusted in the inspector.
    public float xPosBuffer = 10.0f;
    public float yPosBuffer = 10.0f;
    public float zPosBuffer = 10.0f;
    public float rotateBuffer = 30.0f;
    public float scaleBuffer = 6.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = pianoRoot.transform.localPosition;
        float rotate = pianoRoot.transform.localEulerAngles.x;
        float originalScale = pianoRoot.transform.localScale.x;

        // Setting the min and max values of the sliders
        xPos.maxValue = pos.x + xPosBuffer;
        xPos.minValue = pos.x - xPosBuffer;

        yPos.maxValue = pos.y + yPosBuffer;
        yPos.minValue = pos.y - yPosBuffer;

        zPos.maxValue = pos.z + zPosBuffer;
        zPos.minValue = pos.z - zPosBuffer;

        rot.maxValue = rotate + rotateBuffer;
        rot.minValue = rotate - rotateBuffer;

        scale.maxValue = originalScale + scaleBuffer;
        scale.minValue = originalScale - scaleBuffer;

        // Setting the values of the sliders
        xPos.value = pos.x;
        yPos.value = pos.y;
        zPos.value = pos.z;

        rot.value = rotate;
        scale.value = originalScale;
        
    }

    public void UpdatePosition()
    {
        pianoRoot.transform.localPosition = new Vector3(xPos.value, yPos.value, zPos.value);
    }

    public void UpdateRotation()
    {
        pianoRoot.transform.localRotation = Quaternion.Euler(rot.value, 0f, 0f);
    }

    public void UpdateScale()
    {
        pianoRoot.transform.localScale = new Vector3(scale.value, scale.value, scale.value);
    }
}
