using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SonarActivator : MonoBehaviour
{
    public Transform scannerOrigin;
    public Material effectMaterial;
    public float scanDistance;
    public float scanSpeed = 10.0f;

    private Camera rendCamera; 
    //Scannable[] _scannables;

    bool isScanning;

    void Start()
    {
        //_scannables = FindObjectsOfType<Scannable>();
        //Can probably try to have the guards update their minimap icon according to the pings
    }

    void Update()
    {
        if (isScanning)
        {
            scanDistance += Time.deltaTime * scanSpeed;
            //foreach (Scannable s in _scannables) //Probably want to change color for the guards that heard the icon
            //{
            //    if (Vector3.Distance(ScannerOrigin.position, s.transform.position) <= ScanDistance)
            //        s.Ping();
            //}
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    isScanning = true;
        //    scanDistance = 0;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        isScanning = true;
        //        scanDistance = 0;
        //        scannerOrigin.position = hit.point;
        //    }
        //}
    }

    public void PulseSonar(Vector3 origin)
    {
        isScanning = true;
        scanDistance = 0;
        scannerOrigin.position = origin;
    }

    public void hidePulse()
    {
        scanDistance = 10000;
        isScanning = false;
    }

    void OnEnable()
    {
        rendCamera = GetComponent<Camera>();
        rendCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        effectMaterial.SetVector("_WorldSpaceScannerPos", scannerOrigin.position);
        effectMaterial.SetFloat("_ScanDistance", scanDistance);
        RaycastCornerBlit(src, dst, effectMaterial);
    }

    void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = rendCamera.farClipPlane;
        float camFov = rendCamera.fieldOfView;
        float camAspect = rendCamera.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = rendCamera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = rendCamera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (rendCamera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (rendCamera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (rendCamera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (rendCamera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
