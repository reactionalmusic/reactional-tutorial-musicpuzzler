using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public Color color;
    float h = 0;
    float v = 0;

    void Update()
    {
        float colorValue = Reactional.Playback.MusicSystem.GetCurrentBeat() / 16f % 1f;
        color = Color.HSVToRGB(colorValue, 0.5f, 0.8f-(0.1f*colorValue));
        Camera.main.backgroundColor = color;
    }
}
