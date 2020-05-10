using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    [Range(0.0f, 0.05f)] [SerializeField] private float chromAberrAmountX = 0.004f;
    [Range(0.0f, 0.05f)] [SerializeField] private float chromAberrAmountY = 0.004f;
    [Range(0.0f, 10.0f)] [SerializeField] private float rightStripesAmount = 6.18f;
    [Range(0.0f, 1.0f)] [SerializeField] private float rightStripesFill = 0.84f;
    [Range(0.0f, 10.0f)] [SerializeField] private float leftStripesAmount = 6.17f;
    [Range(0.0f, 1.0f)] [SerializeField] private float leftStripesFill = 0.806f;
    [SerializeField] private Vector4 displacementAmount = new Vector4(0.03f, 0.03f, 0.0f, 0.0f);
    [Range(0.0f, 30.0f)] [SerializeField] private float wavyDisplFreq = 22.6f;

    [Range(0.0f, 1.0f)] [SerializeField] private float glitchEffect = 0.0f;
    private Material material;

    public bool activateGlitchEffect = false;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Custom/GlitchEffect"));
    }

    private void Update()
    {
        if (activateGlitchEffect)
        {
            glitchEffect += Time.deltaTime * 0.5f;
            if (glitchEffect >= 1.0f)
                glitchEffect -= 1.0f;
        }
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_ChromAberrAmountX", chromAberrAmountX);
        material.SetFloat("_ChromAberrAmountY", chromAberrAmountY);
        material.SetFloat("_RightStripesAmount", rightStripesAmount);
        material.SetFloat("_RightStripesFill", rightStripesFill);
        material.SetFloat("_LeftStripesAmount", leftStripesAmount);
        material.SetFloat("_LeftStripesFill", leftStripesFill);
        material.SetFloat("_WavyDisplFreq", wavyDisplFreq);
        material.SetVector("_DisplacementAmount", displacementAmount);
        material.SetFloat("_GlitchEffect", glitchEffect);
        Graphics.Blit(source, destination, material);
    }
}
