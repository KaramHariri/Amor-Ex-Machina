using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;
    public static int currentLevelIndex = 0;
    public static bool shouldLoadFromFile = false;

    public GameObject loadingScreen;
    public CanvasGroup canvasGroup;
    public Image progressBarFill;

    float totalSceneProgress = 0.0f;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public static Action loadNextScene = delegate { };

    //SaveSystem
    private int currentSaveFileIndex = 0;
    private int numberOfSaveFiles = 3;
    [SerializeField] private int loadIndex = 0;
    private void OnEnable()
    {
        //loadNextScene += LoadNextScene;
        loadNextScene += StartLoadNextSceneCoroutine;
    }

    private void Awake()
    {
        instance = this;
        canvasGroup.alpha = 0.0f;
        currentLevelIndex = 0;
        SceneManager.LoadSceneAsync((int)currentLevelIndex, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            LoadFromFile();
        }
    }

    public int GetCurrentSaveFileIndex()
    {
        return currentSaveFileIndex;
    }

    public void IncreaseSaveFileIndex()
    {
        currentSaveFileIndex = (currentSaveFileIndex+1) % numberOfSaveFiles;
    }

    //public void LoadNextScene()
    //{
    //    canvasGroup.alpha = 1.0f;
    //    // Adding the operations that we need until the loading is complete
    //    scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelIndex));
    //    currentLevelIndex++;
    //    scenesLoading.Add(SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive));

    //    StartCoroutine("GetSceneLoadProgress");
    //}

    public void StartLoadNextSceneCoroutine()
    {
        StartCoroutine("LoadNextLevel");
    }

    IEnumerator LoadNextLevel()
    {
        shouldLoadFromFile = false;
        canvasGroup.alpha = 1.0f;
        scenesLoading.Add(SceneManager.UnloadSceneAsync(currentLevelIndex));

        yield return new WaitForSeconds(0.5f);

        // Adding the operations that we need until the loading is complete
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

        yield return new WaitForSeconds(0.5f);

        scenesLoading.Add(SceneManager.LoadSceneAsync(currentLevelIndex, LoadSceneMode.Additive));

        StartCoroutine("GetSceneLoadProgress");
    }

    public void LoadFromFile()
    {
        canvasGroup.alpha = 1.0f;
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
            //SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + "test" + (currentSaveFileIndex - 1) + ".save");
            SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + "test" + (loadIndex) + ".save");
            GameEvents.current.LoadDataEvent();
            yield return new WaitForSeconds(0.5f);
        }

        // Wait half a second to load from file
        // LoadFromFile.instance.loadFromFile(path);    // this is just an example

        canvasGroup.alpha = 0.0f;
    }

    private void OnDestroy()
    {
        //loadNextScene -= LoadNextScene;
        loadNextScene -= StartLoadNextSceneCoroutine;
    }
}
