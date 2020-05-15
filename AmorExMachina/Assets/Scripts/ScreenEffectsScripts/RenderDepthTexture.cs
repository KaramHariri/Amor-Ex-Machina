using UnityEngine;

[ExecuteInEditMode]
public class RenderDepthTexture : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
