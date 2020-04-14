using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    bool showScriptableObjects = false;
    PlayerController playerController;

    private void OnEnable()
    {
        playerController = (PlayerController)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        playerController.walkSpeed = EditorGUILayout.FloatField("Walking Speed", playerController.walkSpeed);
        playerController.sneakSpeed = EditorGUILayout.FloatField("Sneaking Speed", playerController.sneakSpeed);
        playerController.rotateVelocity = EditorGUILayout.FloatField("Rotate Velocity", playerController.rotateVelocity);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showScriptableObjects = EditorGUILayout.Foldout(showScriptableObjects, "Show Scriptable Objects");
        if (showScriptableObjects)
        {
            playerController.gameStateSubject = (GameStateSubject)EditorGUILayout.ObjectField("Game State Subject", playerController.gameStateSubject, typeof(object), true);
            playerController.playerSoundSubject = (PlayerSoundSubject)EditorGUILayout.ObjectField("Player Sound Subject", playerController.playerSoundSubject, typeof(object), true);
            playerController.guardHackedSubject = (GuardHackedSubject)EditorGUILayout.ObjectField("Guard Hacked Subject", playerController.guardHackedSubject, typeof(object), true);
            playerController.playerVariables = (PlayerVariables)EditorGUILayout.ObjectField("Player Variables", playerController.playerVariables, typeof(object), true);
            playerController.cameraSwitchedToFirstPerson = (BoolVariable)EditorGUILayout.ObjectField("Camera Switched To First Person", playerController.cameraSwitchedToFirstPerson, typeof(object), true);
            playerController.firstPersonCameraVariables = (FirstPersonCameraVariables)EditorGUILayout.ObjectField("First Person Camera Variables", playerController.firstPersonCameraVariables, typeof(object), true);
            playerController.thirdPersonCameraVariables = (ThirdPersonCameraVariables)EditorGUILayout.ObjectField("Third Person Camera Variables", playerController.thirdPersonCameraVariables, typeof(object), true);
            playerController.firstPersonCameraTransform = (TransformVariable)EditorGUILayout.ObjectField("First Person Camera Transform", playerController.firstPersonCameraTransform, typeof(object), true);
            playerController.thirdPersonCameraTransform = (TransformVariable)EditorGUILayout.ObjectField("Third Person Camera Transform", playerController.thirdPersonCameraTransform, typeof(object), true);
            playerController.PlayerLastSightPositionSubject = (PlayerLastSightPositionSubject)EditorGUILayout.ObjectField("Player Last Sight Position Subject", playerController.PlayerLastSightPositionSubject, typeof(object), true);
        }

        EditorGUILayout.EndVertical();
    }
}
