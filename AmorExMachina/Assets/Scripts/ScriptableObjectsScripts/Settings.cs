using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Settings", order = 60)]
public class Settings : ScriptableObject
{
    public bool invertY;
    public bool subtitle;
    [Range(50.0f, 300.0f)]
    public float thirdPersonLookSensitivity;
    [Range(50.0f, 300.0f)]
    public float firstPersonLookSensitivity;
    [Range(0.0f, 1.0f)]
    public float effectsVolume;
    [Range(0.0f, 1.0f)]
    public float footstepsVolume;
    [Range(0.0f, 1.0f)]
    public float voiceVolume;
    [Range(0.0f, 1.0f)]
    public float musicVolume;


    [Header("Controls")]
    [Header("Keyboard")]
    public KeyCode rotatePuzzleArrowKeyboard = KeyCode.F;
    public KeyCode activateButtonInPuzzleKeyboard = KeyCode.E;
    public KeyCode cameraToggleKeyboard = KeyCode.Q;
    public KeyCode movementToggleKeyboard = KeyCode.LeftShift;
    public KeyCode disableGuardKeyboard = KeyCode.E;
    public KeyCode hackGuardKeyboard = KeyCode.R;
    public KeyCode distractGuardWhileHackingKeyboard = KeyCode.E;

    public KeyCode defaultRotatePuzzleArrowKeyboard = KeyCode.F;
    public KeyCode defaultActivateButtonInPuzzleKeyboard = KeyCode.E;
    public KeyCode defaultCameraToggleKeyboard = KeyCode.Q;
    public KeyCode defaultMovementToggleKeyboard = KeyCode.LeftShift;
    public KeyCode defaultDisableGuardKeyboard = KeyCode.E;
    public KeyCode defaultHackGuardKeyboard = KeyCode.R;
    public KeyCode defaultDistractGuardWhileHackingKeyboard = KeyCode.E;

    [Header("Controller")]
    public KeyCode rotatePuzzleArrowController = KeyCode.JoystickButton3;
    public KeyCode activateButtonInPuzzleController = KeyCode.JoystickButton1;
    public KeyCode cameraToggleController = KeyCode.JoystickButton11;
    public KeyCode movementToggleController = KeyCode.JoystickButton10;
    public KeyCode disableGuardController = KeyCode.JoystickButton1;
    public KeyCode hackGuardController = KeyCode.JoystickButton0;
    public KeyCode distractGuardWhileHackingController = KeyCode.JoystickButton1;

    public KeyCode defaultRotatePuzzleArrowController = KeyCode.JoystickButton3;
    public KeyCode defaultActivateButtonInPuzzleController = KeyCode.JoystickButton1;
    public KeyCode defaultCameraToggleController = KeyCode.JoystickButton11;
    public KeyCode defaultMovementToggleController = KeyCode.JoystickButton10;
    public KeyCode defaultDisableGuardController = KeyCode.JoystickButton1;
    public KeyCode defaultHackGuardController = KeyCode.JoystickButton0;
    public KeyCode defaultDistractGuardWhileHackingController = KeyCode.JoystickButton1;

    [Header("Controls Strings")]
    public string rotatePuzzleArrow = "RotatePuzzleArrow";
    public string activateButtonInPuzzle = "ActivateButtonInPuzzle";
    public string cameraToggle = "CameraToggle";
    public string movementToggle = "MovementToggle";
    public string disableGuard = "DisableGuard";
    public string hackGuard = "HackGuard";
    public string distractGuardWhileHacking = "DistactGuardWhileHacking";
}
