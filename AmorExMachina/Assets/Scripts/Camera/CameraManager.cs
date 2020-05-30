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

    //public PlayerCamerasVariables playerCamerasVariables = null;
    //public GuardHackedSubject guardHackedSubject = null;
    //public Settings settings = null;
    //private AudioManager audioManager = null;

    private PlayerCamerasVariables playerCamerasVariables = null;
    private GuardHackedSubject guardHackedSubject = null;
    private Settings settings = null;
    private AudioManager audioManager = null;

    private RadialBlurEffect radialBlurEffect = null;

    private void Start()
    {
        Camera mainCamera = Camera.main;
        cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        radialBlurEffect = mainCamera.GetComponent<RadialBlurEffect>();
        switchedToFirstPersonCamera = false;
        //playerCameras.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
        AddGuardVirtualCamerasToDictionary();
        //guardHackedSubject.AddObserver(this);

        //audioManager = FindObjectOfType<AudioManager>();

        //Added 20-05-18
        playerCamerasVariables = GameHandler.playerCamerasVariables;
        if(playerCamerasVariables == null)
        {
            Debug.Log("CameraManager can't find PlayerCamerasVariables in GameHandler");
        }
        playerCamerasVariables.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
        audioManager = GameHandler.audioManager;
        if(audioManager == null)
        {
            Debug.Log("CameraManager can't find AudioManager in GameHandler");
        }
        guardHackedSubject = GameHandler.guardHackedSubject;
        if(guardHackedSubject == null)
        {
            Debug.Log("CameraManager can't find GuardHackedSubject in GameHandler");
        }
        guardHackedSubject.AddObserver(this);

        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("CameraManager can't find Settings in GameHandler");
        }

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
            if (ThirdPersonCinemachine.thirdPersonCamera.m_Priority == 22)
            {
                audioManager.Play("SwitchCameraToFirstPerson");
                ThirdPersonCinemachine.thirdPersonCamera.m_Priority = 18;
                StartCoroutine("ResetCamera");
            }
            else if (ThirdPersonCinemachine.thirdPersonCamera.m_Priority == 18)
            {
                audioManager.Play("SwitchCameraToThirdPerson");
                ThirdPersonCinemachine.thirdPersonCamera.m_Priority = 22;
                switchedToFirstPersonCamera = false;
                playerCamerasVariables.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
                StopCoroutine("ResetCamera");
            }
        }
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);
        switchedToFirstPersonCamera = true;
        playerCamerasVariables.switchedCameraToFirstPerson = switchedToFirstPersonCamera;
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
                    yield return null;
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