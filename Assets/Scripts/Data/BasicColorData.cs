using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BasicColorData : UpdateableData
{
    public BasicTerrainType[] regions;
}

[System.Serializable]
public struct BasicTerrainType
{
    public string name;
    public float height;
    public Color color;
}
