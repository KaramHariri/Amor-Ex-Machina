using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour, IGuardHackedObserver
{
    private Dictionary<string, CinemachineVirtualCamera> guardsVirtualCameras = new Dictionary<string, CinemachineVirtualCamera>();
    string hackedGuardName = "";
    private bool switchedToFirstPersonCamera = false;
    private CinemachineBrain cinemachineBrain = null;

    public PlayerCamerasVariables playerCameras = null;
    public GuardHackedSubject guardHackedSubject = null;
    public Settings settings = null;
    private AudioManager audioManager = null;

    private RadialBlurEffect radialBlurEffect = null;

    private void Start()
    {
        Camera mainCamera = Camera.main;
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        radialBlurEffect = mainCamera.GetComponent<RadialBlurEffect>();
        switchedToFirstPersonCamera = false;
        playerCameras.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
        AddGuardVirtualCamerasToDictionary();
        guardHackedSubject.AddObserver(this);

        audioManager = FindObjectOfType<AudioManager>();

        StartCoroutine("ControllerCheck");
    }

    private void Update()
    {
        if (GameHandler.currentState != GameState.NORMALGAME) { return; }
        SwitchPlayerCameraCheck();
        ActivateBlurCheck();
    }

    void SwitchPlayerCameraCheck()
    {
        bool switching = false;
        if(Input.GetKeyDown(settings.cameraToggleKeyboard) || Input.GetKeyDown(settings.cameraToggleController))
        {
            switching = true;
        }

        if (switching)
        {
            if (playerCameras.thirdPersonCamera.m_Priority == 22)
            {
                audioManager.Play("SwitchCameraToFirstPerson");
                playerCameras.thirdPersonCamera.m_Priority = 18;
                StartCoroutine("ResetCamera");
            }
            else if (playerCameras.thirdPersonCamera.m_Priority == 18)
            {
                audioManager.Play("SwitchCameraToThirdPerson");
                playerCameras.thirdPersonCamera.m_Priority = 22;
                switchedToFirstPersonCamera = false;
                playerCameras.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
                StopCoroutine("ResetCamera");
            }
        }
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);
        switchedToFirstPersonCamera = true;
        playerCameras.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
    }

    void AddGuardVirtualCamerasToDictionary()
    {
        for(int i = 0; i < GameHandler.guards.Length; i++)
        {
            guardsVirtualCameras.Add(GameHandler.guards[i].name, GameHandler.guards[i].vC);
        }
    }

    void ActivateBlurCheck()
    {
        if (cinemachineBrain.IsBlending)
        {
            radialBlurEffect.blurAmount = Mathf.Lerp(radialBlurEffect.blurAmount, radialBlurEffect.maxBlurAmount, Time.deltaTime * 2.0f);
        }
        else
        {
            radialBlurEffect.blurAmount = Mathf.Lerp(radialBlurEffect.blurAmount, 0.0f, Time.deltaTime * radialBlurEffect.blurFadeOutSpeed);
        }
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
                        settings.useControllerInput = true;
                    }
                    else
                    {
                        settings.useControllerInput = false;
                    }
                }
            }
            else if(temp.Length <= 0)
            {
                settings.useControllerInput = false;
            }
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator StartActivatingBlurEffect()
    {
        ActivateBlurCheck();
        yield return null;
    }

    public void GuardHackedNotify(string guardName)
    {
        if (guardName != "")
        {
            hackedGuardName = guardName;
            guardsVirtualCameras[guardName].m_Priority = 23;
        }
        else if(guardName == "" && hackedGuardName != "")
        {
            guardsVirtualCameras[hackedGuardName].m_Priority = 15;
            hackedGuardName = "";
        }
    }

    void OnDestroy()
    {
        guardHackedSubject.RemoveObserver(this);
    }
}