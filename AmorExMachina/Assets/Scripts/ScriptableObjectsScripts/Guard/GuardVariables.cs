using UnityEngine;

[CreateAssetMenu(fileName = "GuardVariables", menuName = "Guard/GuardVariables", order = 50)]
public class GuardVariables : ScriptableObject
{
    [Header("Field Of View")]
    public float fieldOfViewAngle = 45.0f;
    public float fieldOfViewRadius = 20.0f;
    public float maxBackupRadius = 40.0f;

    [Header("Speed")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float suspiciousSpeed = 3.5f;

    [Header("Timers")]
    public float maxIdletimer = 5.0f;
    public float maxTalkingTimer = 5.0f;

    [Header("Material Color (Prototype)")]
    public Color disabledColor = Color.black;
    public Color chasingColor = Color.red;
    public Color suspiciousColor = Color.yellow;
    public Color patrolColor = Color.green;
    public Color stationaryColor = Color.blue;
    public Color idleColor = Color.cyan;
}
