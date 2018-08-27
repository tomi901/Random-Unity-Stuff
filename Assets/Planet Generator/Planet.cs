using UnityEngine;


public class Planet : MonoBehaviour
{

    static readonly Vector3[] directions =
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    [Range(TerrainFace.minResolution, TerrainFace.maxResolution)]
    public int resolution = 2;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[directions.Length];
        }
        terrainFaces = new TerrainFace[directions.Length];

        Material planetMaterial = new Material(Shader.Find("Standard"));

        for (int i = 0; i < directions.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("Terrain face " + directions[i]);
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = planetMaterial;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            if (terrainFaces[i] == null)
            {
                terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i]);
            }
            else terrainFaces[i].Resolution = resolution;
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

}
