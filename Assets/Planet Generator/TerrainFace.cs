using UnityEngine;


public class TerrainFace
{

    public const int minResolution = 2;
    public const int maxResolution = 256;

    private readonly Mesh mesh;

    private int resolution = 2;
    public int Resolution
    {
        get { return resolution; }
        set
        {
            resolution = Mathf.Clamp(value, minResolution, maxResolution);
        }
    }

    private readonly Vector3 localUp;
    private readonly Vector3 tangent;
    private readonly Vector3 biTangent;

    private readonly ShapeGenerator shapeGenerator;


    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;

        this.Resolution = resolution;
        resolution = this.Resolution;

        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        tangent = new Vector3(localUp.y, localUp.z, localUp.x);
        biTangent = Vector3.Cross(localUp, tangent);
    }

    public void ConstructMesh()
    {
        int faceResolution = resolution - 1;

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[faceResolution * faceResolution * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / faceResolution;
                Vector3 pointOnUnitCube = localUp + 
                    (percent.x - .5f) * 2 * tangent +
                    (percent.y - .5f) * 2 * biTangent;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitCube.normalized);

                if (x < faceResolution && y < faceResolution)
                {
                    triangles[triIndex++] = i;
                    triangles[triIndex++] = i + resolution + 1;
                    triangles[triIndex++] = i + resolution;

                    triangles[triIndex++] = i;
                    triangles[triIndex++] = i + 1;
                    triangles[triIndex++] = i + resolution + 1;
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }
    }
	
}
