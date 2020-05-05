using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] OptionsMenu optionsMenu = null;
    [SerializeField] EventSystem eventSystem = null;

    GameObject currentSelectedButton = null;

    private void Start()
    {
        StartCoroutine("DeactivateBackground");
    }

    public void NewGameButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        Debug.Log("Load Game Pressed !");
    }

    public void OptionsButton()
    {
        StartCoroutine("SwitchToOptions");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    IEnumerator DeactivateBackground()
    {
        yield return new WaitForSeconds(1.0f);
            transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator SwitchToOptions()
    {
        optionsMenu.gameObject.SetActive(true);
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(optionsMenu.firstSelectedButtonInOptions);
        transform.gameObject.SetActive(false);
    }

    public void StartSetSelectedButtonEnumerator()
    {
        StartCoroutine("SetSelectedButton");
    }

    IEnumerator SetSelectedButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(currentSelectedButton);
    }
}
