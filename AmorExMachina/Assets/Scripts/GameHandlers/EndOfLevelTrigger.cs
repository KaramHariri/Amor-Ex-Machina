using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelTrigger : MonoBehaviour
{
    private SceneHandler sceneHandlerInstance = null;

    private void Awake()
    {
        sceneHandlerInstance = SceneHandler.instance;
    }

    //IEnumerator LoadNextScene()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    SceneHandler.loadNextScene();
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //StartCoroutine("LoadNextScene");
            SceneHandler.loadNextScene();
        }
    }
}
