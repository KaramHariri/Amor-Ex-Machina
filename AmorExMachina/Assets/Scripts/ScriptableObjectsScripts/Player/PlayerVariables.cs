using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Player/PlayerVariables", order = 52)]
public class PlayerVariables : ScriptableObject
{
    public Transform playerTransform;
    public bool caught = false;
    public bool canHackGuard = true;
}
