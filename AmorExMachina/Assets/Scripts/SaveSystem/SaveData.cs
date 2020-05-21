using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();
            }

            return _current;
        }

        set
        {
            if (value != null)
            {
                _current = value;
            }
        }
    }

    public List<GuardData> guardData = new List<GuardData>();
    public PlayerData playerData = new PlayerData();
    public Dictionary<string, GuardData> guards = new Dictionary<string, GuardData>();
    public Dictionary<string, CheckpointData> checkpoints = new Dictionary<string, CheckpointData>();

    public Dictionary<string, DoorData> doors = new Dictionary<string, DoorData>();

}
