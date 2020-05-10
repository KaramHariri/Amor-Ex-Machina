using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInstanceDebugger : MonoBehaviour
{
    public List<GuardData> guardData = new List<GuardData>();
    public Dictionary<string, GuardData> guards = new Dictionary<string, GuardData>();

    private float cd = 3.0f;
    void Update()
    {
        cd -= Time.deltaTime;

        if(cd <= 0f)
        {
            guardData = SaveData.current.guardData;
            guards = SaveData.current.guards;
            Debug.Log("guards.Count :" + guards.Count);
            Debug.Log("checkpoint count :" + SaveData.current.checkpoints.Count);
            cd = 3.0f;
        }
    }
}
