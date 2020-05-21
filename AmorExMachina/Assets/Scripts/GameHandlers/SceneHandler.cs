using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;
    public static int currentLevelIndex = 1;
    public static bool shouldLoadFromFile = false;

    public GameObject loadingScreen;
    public CanvasGroup canvasGroup;
    public Image progressBarFill;

    float totalSceneProgress = 0.0f;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public static Action loadNextScene = delegate { };

    public static bool hasSaveToAFile = false;

    private void OnEnable()
    {
        loadNextScene += StartLoadNextSceneCoroutine;
    }

    private void Awake()
    {
        instance = this;
        canvasGroup.alpha = 0.0f;
        currentLevelIndex = 1;
        SceneManager.LoadSceneAsync((int)currentLevelIndex, LoadSceneMode.Additive);
    }

    public void StartLoadNextSceneCoroutine()
    {
        StartCoroutine("LoadNextLevel");
    }

    IEnumerator LoadNextLevel()
    {
        shouldLoadFromFile = false;
        hasSaveToAFile = false;
        canvasGroup.alpha = 1.0f;
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelIndex));

        yield return new WaitForSeconds(0.5f);

        currentLevelIndex++;
        scenesLoading.Add(SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive));

        StartCoroutine("GetSceneLoadProgress");
    }

    public void StartReloadSceneCoroutine()
    {
        StartCoroutine("ReloadScene");
    }

    IEnumerator ReloadScene()
    {
        canvasGroup.alpha = 1.0f;
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelIndex));
        shouldLoadFromFile = false;
        yield return new WaitForSeconds(0.5f);

        scenesLoading.Add(SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive));

        StartCoroutine("GetSceneLoadProgress");
    }

    public void LoadFromFile()
    {
        canvasGroup.alpha = 1.0f;
        shouldLoadFromFile = true;
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelIndex));
        scenesLoading.Add(SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive));

        StartCoroutine("GetSceneLoadProgress");
    }
   
    IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;
                foreach (AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100.0f;
                float progressAmount = totalSceneProgress / 100.0f;
                progressBarFill.fillAmount = Mathf.RoundToInt(progressAmount);
                yield return null;
            }
        }

        if(shouldLoadFromFile == true)
        {
            SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + "test" + ".save");
            yield return new WaitForSeconds(0.5f);
            GameEvents.current.LoadDataEvent();
        }

        canvasGroup.alpha = 0.0f;
    }

    private void OnDestroy()
    {
        loadNextScene -= StartLoadNextSceneCoroutine;
    }
}
