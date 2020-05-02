using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Settings", order = 60)]
public class Settings : ScriptableObject
{
    public bool invertY;
    public bool subtitle;
    [Range(50.0f, 300.0f)]
    public float thirdPersonLookSensitivity;
    [Range(50.0f, 300.0f)]
    public float firstPersonLookSensitivity;
    [Range(0.0f, 1.0f)]
    public float effectsVolume;
    [Range(0.0f, 1.0f)]
    public float footstepsVolume;
    [Range(0.0f, 1.0f)]
    public float voiceVolume;
    [Range(0.0f, 1.0f)]
    public float musicVolume;
}
