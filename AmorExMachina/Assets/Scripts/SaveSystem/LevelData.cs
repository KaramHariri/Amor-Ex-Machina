using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData 
{
    public List<GuardData> guardData = new List<GuardData>();
    public PlayerData playerData = new PlayerData();
}
