using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Intro : MonoBehaviour
{
    private VideoPlayer videoPlayer = null;
    [SerializeField] private float videoPlayDelay = 0.2f;
    Sprite Xbutton = null;
    Sprite Ebutton = null;
    [SerializeField] Image maskImage = null;
    [SerializeField] Image fillImage = null;
    [SerializeField] private float maxTimerToSkip = 2.0f;
    private float currentTimerToSkip = 0.0f;

    SceneHandler sceneHandler = null;

    private bool usingController = false;

    private bool hasPlayed = false;

    void Start()
    {
        Xbutton = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/XButton");
        Ebutton = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/buttons_kb_e_round");
        videoPlayer = GetComponent<VideoPlayer>();
        StartCoroutine("PrepareVideo");
        StartCoroutine("ControllerCheck");
        sceneHandler = SceneHandler.instance;
    }

    private void Update()
    {
        if(hasPlayed && !videoPlayer.isPlaying)
        {
            if(sceneHandler != null)
            {
                sceneHandler.StartLoadNextSceneCoroutine();
            }
        }

        if(usingController)
        {
            maskImage.sprite = Xbutton;
        }
        else
        {
            maskImage.sprite = Ebutton;
        }

        if(Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.E))
        {
            currentTimerToSkip += Time.deltaTime;
            if(currentTimerToSkip >= maxTimerToSkip - 0.1f)
            {
                currentTimerToSkip = maxTimerToSkip;
                if(sceneHandler != null)
                {
                    sceneHandler.StartLoadNextSceneCoroutine();
                }
            }
        }
        else
        {
            currentTimerToSkip -= Time.deltaTime * 2.0f;
        }

        fillImage.fillAmount = currentTimerToSkip / maxTimerToSkip;
        currentTimerToSkip = Mathf.Clamp(currentTimerToSkip, 0, maxTimerToSkip);
    }

    IEnumerator ControllerCheck()
    {
        while (true)
        {
            string[] temp = Input.GetJoystickNames();

            if (temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(temp[i]))
                    {
                        usingController = true;
                    }
                    else
                    {
                        usingController = false;
                    }
                    yield return null;
                }
            }
            else if (temp.Length <= 0)
            {
                usingController = false;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator PrepareVideo()
    {
        videoPlayer.Prepare();
        while(!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();
        yield return new WaitForSeconds(videoPlayDelay);
        transform.GetChild(0).gameObject.SetActive(false);
        hasPlayed = true;
    }
}
