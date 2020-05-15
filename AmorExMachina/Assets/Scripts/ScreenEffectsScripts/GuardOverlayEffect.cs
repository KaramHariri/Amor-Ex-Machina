using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardOverlayEffect : MonoBehaviour
{
    private Material material;

    void Awake()
    {
        material = new Material(Resources.Load<Shader>("Shaders/GuardScreenOverlay"));
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
