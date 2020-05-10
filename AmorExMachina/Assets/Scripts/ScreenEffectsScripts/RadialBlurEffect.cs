using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBlurEffect : MonoBehaviour
{
    [Range(0.0f, 0.5f)] public float maxBlurAmount = 0.25f;
    [HideInInspector] public float blurAmount = 0.0f;
    [Range(0.0f, 1.0f)] [SerializeField] private float blurRadius = 0.75f;
    [Range(1.0f, 10.0f)] public float blurFadeOutSpeed = 4.0f;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Custom/RadialBlur"));
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_Radius", blurRadius);
        material.SetFloat("_EffectAmount", blurAmount);
        Graphics.Blit(source, destination, material);
    }
}
