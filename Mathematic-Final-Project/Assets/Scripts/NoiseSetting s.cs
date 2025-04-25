using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NoiseSettings
{
    [Range(0.001f , 1f)]
    public float Frequency = 0.01f;
    
    [Range(0f , 10f)]
    public float Amplitude = 1f;
    
}
