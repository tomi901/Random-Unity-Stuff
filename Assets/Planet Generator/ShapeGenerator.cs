using UnityEngine;


public class ShapeGenerator
{

    private readonly ShapeSettings settings;
    private readonly NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;

        var layers = settings.GetNoiseLayers;
        noiseFilters = new NoiseFilter[layers.Count];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new NoiseFilter(layers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = 0;

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.LayerEnabled(i))
            {
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere);
            }
        }

        return pointOnUnitSphere * settings.GetRadius * (elevation + 1f);
    }
	
}
