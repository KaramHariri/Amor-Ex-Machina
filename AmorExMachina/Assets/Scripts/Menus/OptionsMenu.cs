using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    MainMenu mainMenu;
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    CanvasGroup audioCanvasGroup = null;
    [SerializeField]
    CanvasGroup gameplayCanvasGroup = null;
    [SerializeField]
    CanvasGroup buttonsCanvasGroup = null;
    public GameObject firstSelectedButtonInOptions = null;
    public GameObject firstSelectedButtonInAudio = null;
    public GameObject firstSelectedButtonInGameplay = null;

    GameObject currentSelectedButton = null;

    [SerializeField]
    private float fadingSpeed = 4.0f;
    [SerializeField]
    private bool inAudioMenu = false;
    [SerializeField]
    private bool inGameplayMenu = false;

    private void Start()
    {
        transform.gameObject.SetActive(false);
        audioCanvasGroup.alpha = 0.0f;
        gameplayCanvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (inAudioMenu)
            {
                Debug.Log("Switching from audio");
                StartSwitchingFromAudioCoroutine();
                return;
            }
            else if (inGameplayMenu)
            {
                Debug.Log("Switching from gameplay");
                StartSwitchingFromGameplayCoroutine();
                return;
            }
            else
            {
                ExitOptionsMenu();
            }
        }
    }

    public void ExitOptionsMenu()
    {
        StartCoroutine("SwitchToMainMenu");
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

    IEnumerator SwitchToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        yield return null;
        transform.gameObject.SetActive(false);
        mainMenu.StartSetSelectedButtonEnumerator();
    }

    public void StartSwitchingFromGameplayCoroutine()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(gameplayCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        inGameplayMenu = false;
    }

    public void StartSwitchingFromAudioCoroutine()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(audioCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        inAudioMenu = false;
    }

    public void StartSwitchingToAudioMenuCoroutine()
    {
        StartCoroutine(UpdateCurrentSelectedObject(firstSelectedButtonInAudio));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(audioCanvasGroup)));
        inAudioMenu = true;
    }

    public void StartSwitchingToGameplayCoroutine()
    {
        StartCoroutine(UpdateCurrentSelectedObject(firstSelectedButtonInGameplay));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(gameplayCanvasGroup)));
        inGameplayMenu = true;
    }

    IEnumerator SwitchOptionMenu(IEnumerator fadeOutEnumerator, IEnumerator fadeInEnumerator)
    {
        StartCoroutine(fadeOutEnumerator);
        yield return null;
        StartCoroutine(fadeInEnumerator);
    }

    IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroupToFade)
    {
        while(canvasGroupToFade.alpha > 0.0f)
        {
            canvasGroupToFade.alpha -= Time.deltaTime * fadingSpeed;
            yield return null;
        }
    }

    IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroupToFade)
    {
        while (canvasGroupToFade.alpha < 1.0f)
        {
            canvasGroupToFade.alpha += Time.deltaTime * fadingSpeed;
            yield return null;
        }
    }

    IEnumerator UpdateCurrentSelectedObject(GameObject nextSelectedObject)
    {
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(nextSelectedObject);
    }
}
