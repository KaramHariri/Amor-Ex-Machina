using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float rotateVelocity = 100.0f;
    private float verticalInput = 0.0f;
    private float horizontalInput = 0.0f;
    private float moveAmount = 0.0f;

    private Rigidbody rb = null;
    private Camera mainCamera = null;
    private Guard guard = null;

    private PlayerSoundSubject playerSoundSubject = null;
    private Settings settings = null;

    private AudioManager audioManager = null;

    private ParticleSystem distractionParticleSystem = null;

    //Added 2020-05-20
    [Header("Distraction")]
    [SerializeField] private float distractionCooldown = 5.0f;
    [SerializeField] private float distractionTimer = 0.0f;

    //Added 2020-05-26
    private AudioSource rightFootSound = null;
    private AudioSource leftFootSound = null;
    float walkingStepDistance = 0.8f;
    //float runningStepDistance = 0.4f;
    private float accumulateDistance = 0.0f;
    private bool soundFromRightFoot = true;

    //Added 2020-05-27
    private GameObject AudioListenerObject = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        guard = GetComponent<Guard>();
        mainCamera = Camera.main;
        distractionParticleSystem = transform.Find("VFX").Find("Guard Distraction").GetComponent<ParticleSystem>();
        rightFootSound = transform.Find("RightFootAudioSource").GetComponent<AudioSource>();
        leftFootSound = transform.Find("LeftFootAudioSource").GetComponent<AudioSource>();
        AudioListenerObject = GameObject.Find("AudioListener");
    }

    private void Start()
    {
        GetStaticReferencesFromGameHandler();
    }

    void GetStaticReferencesFromGameHandler()
    {
        playerSoundSubject = GameHandler.playerSoundSubject;
        if (playerSoundSubject == null)
        {
            Debug.Log("GuardController can't find PlayerSoundSubject in GameHandler");
        }

        settings = GameHandler.settings;
        if (settings == null)
        {
            Debug.Log("GuardController can't find Settings in GameHandler");
        }

        audioManager = GameHandler.audioManager;
        if(audioManager == null)
        {
            Debug.Log("GuardController can't find AudioManager in GameHandler");
        }
    }

    void Update()
    {
        if (guard.hacked)
        {
            rb.isKinematic = false;
        }
        else
        {
            rb.isKinematic = true;
        }

        if (GameHandler.currentState != GameState.HACKING) { return; }
        distractionTimer -= Time.deltaTime;
        if (distractionTimer < 0)
            distractionTimer = 0;

        if(GameHandler.currentState == GameState.MENU) { return; }

        if (guard.disabled && guard.hacked)
        {
            GetInput();
        }

        PlaySound();
    }

    void FixedUpdate()
    {
        if (GameHandler.currentState == GameState.MENU) { return; }

        if (guard.disabled && guard.hacked)
        {
            HandleRotation();
            HandleMovement();
        }
    }

    private void LateUpdate()
    {
        MoveAudioListener();
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        if(distractionTimer <= 0 && (Input.GetKeyDown(settings.distractGuardWhileHackingController) || Input.GetKeyDown(settings.distractGuardWhileHackingKeyboard)))
        {
            playerSoundSubject.NotifyObservers(SoundType.DISTRACTION, this.transform.position);
            PlayDistractionParticleSystem();
            audioManager.Play("DistractGuard", this.transform.position);
            distractionTimer = distractionCooldown;
        }
    }

    void HandleRotation()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        Quaternion targetRotation = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
        Vector3 v = transform.forward;
        Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
        v = dir;
        v.Normalize();
        v *= walkSpeed * moveAmount;

        v.y = rb.velocity.y;
        rb.velocity = v;

        if(v.magnitude > 0)
        {
            accumulateDistance += Time.deltaTime;
        }
        else
        {
            accumulateDistance = 0.0f;
        }
    }

    void PlayDistractionParticleSystem()
    {
        distractionParticleSystem.Play();
    }

    void PlaySound()
    {
        if (accumulateDistance > walkingStepDistance)
        {
            if (soundFromRightFoot)
            {
                rightFootSound.Play();
            }
            else
            {
                leftFootSound.Play();
            }

            accumulateDistance = 0.0f;
            soundFromRightFoot = !soundFromRightFoot;
        }
    }

    private void MoveAudioListener()
    {
        if(guard.hacked)
        {
            AudioListenerObject.transform.position = this.transform.position;
        }
    }
}