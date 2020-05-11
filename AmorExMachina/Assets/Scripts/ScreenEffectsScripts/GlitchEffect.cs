using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchEffect : MonoBehaviour
{
    private float chromAberrAmountX = 0.004f;
    private float chromAberrAmountY = 0.004f;
    private Vector4 displacementAmount = new Vector4(0.03f, 0.03f, 0.0f, 0.0f);
    private float wavyDisplFreq = 22.6f;
    [HideInInspector] public float rightStripesAmount = 6.18f;
    [HideInInspector] public float rightStripesFill = 0.84f;
    [HideInInspector] public float leftStripesAmount = 6.17f;
    [HideInInspector] public float leftStripesFill = 0.806f;

    [SerializeField] private float glitchEffect = 0.0f;
    private Material material;

    [HideInInspector] public bool activateGlitchEffect = false;
    [HideInInspector] public float glitchUpdateSpeed = 0.5f;
    [HideInInspector] public float minDisplacmentAmount = 0.03f;
    [HideInInspector] public float maxDisplacmentAmount = 0.08f;

    float timer = 0.0f;

    // Creates a private material used to the effect
    void Awake()
    {
        //material = new Material(Shader.Find("Custom/GlitchEffect"));
        material = new Material(Resources.Load<Shader>("Shaders/GlitchEffect"));
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (activateGlitchEffect)
        {
            if (timer >= 1.0f)
            {
                displacementAmount.x = Random.Range(minDisplacmentAmount, maxDisplacmentAmount);
                displacementAmount.y = Random.Range(minDisplacmentAmount, maxDisplacmentAmount);
                timer = 0.0f;
            }
            glitchEffect += Time.deltaTime * glitchUpdateSpeed;
            if (glitchEffect >= 1.0f)
                glitchEffect -= 1.0f;
        }
        else
            glitchEffect = 0.0f;
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
