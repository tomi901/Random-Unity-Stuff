using System;
using System.Collections.ObjectModel;
using UnityEngine;


[CreateAssetMenu(fileName = "New Shape Settings", menuName = "Planet Settings/Shape")]
public class ShapeSettings : ScriptableObject
{

    [SerializeField]
    private float radius = 1f;
    public float GetRadius { get { return radius; } }

    [SerializeField]
    private NoiseLayer[] noiseLayers;
    public ReadOnlyCollection<NoiseLayer> GetNoiseLayers { get { return Array.AsReadOnly(noiseLayers); } }

    public bool LayerEnabled(int index)
    {
        return noiseLayers[index].enabled;
    }

    [Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }

}

[Serializable]
public class NoiseSettings
{

    public float strength = 1;
    [Range(1, 8)]
    public int numLayers = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistence = .5f;
    public Vector3 centre;
    public float minValue;
}