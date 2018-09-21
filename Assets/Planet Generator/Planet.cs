using UnityEngine;


public class Planet : MonoBehaviour
{

    private static readonly Vector3[] directions =
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    [Range(TerrainFace.minResolution, TerrainFace.maxResolution), SerializeField]
    private int resolution = 2;
    [SerializeField]
    private bool autoUpdate = false;

    [SerializeField]
    private ColorSettings colorSettings;
    public ColorSettings GetColorSettings { get { return colorSettings; } }

    [SerializeField]
    private ShapeSettings shapeSettings;
    public ShapeSettings GetShapeSettings { get { return shapeSettings; } }

    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    private ShapeGenerator shapeGenerator;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

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
                terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            }
            else terrainFaces[i].Resolution = resolution;
        }
    }


    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }


    private void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    private void GenerateColors()
    {
        if (colorSettings == null) return;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.GetColor;
        }
    }

}
