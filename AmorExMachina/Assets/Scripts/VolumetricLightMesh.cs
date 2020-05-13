using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(MeshFilter))]
public class VolumetricLightMesh : MonoBehaviour
{
    private Light spotLight;
    private MeshFilter meshFilter;

    private Mesh mesh;

    [SerializeField] private float maximumOpacity = 0.25f;

    void Start()
    {
        spotLight = GetComponent<Light>();
        meshFilter = GetComponent<MeshFilter>();
        BuildMesh();
    }

    void BuildMesh()
    {
        mesh = new Mesh();
        float farPosition = Mathf.Tan(spotLight.spotAngle * 0.5f * Mathf.Deg2Rad) * spotLight.range;
        mesh.vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(farPosition, farPosition, spotLight.range),
            new Vector3(-farPosition, farPosition, spotLight.range),
            new Vector3(-farPosition, -farPosition, spotLight.range),
            new Vector3(farPosition, -farPosition, spotLight.range)
        };

        mesh.colors = new Color[]
        {
            new Color(spotLight.color.r,spotLight.color.g,spotLight.color.b,spotLight.color.a * maximumOpacity),
            new Color(spotLight.color.r,spotLight.color.g,spotLight.color.b,0f),
            new Color(spotLight.color.r,spotLight.color.g,spotLight.color.b,0f),
            new Color(spotLight.color.r,spotLight.color.g,spotLight.color.b,0f),
            new Color(spotLight.color.r,spotLight.color.g,spotLight.color.b,0f)
        };

        mesh.triangles = new int[]
        {
            0,1,2,
            0,2,3,
            0,3,4,
            0,4,1
        };

        meshFilter.mesh = mesh;
    }
}
