using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Divide campaign map into grid
// Assign 2-3 custom maps to each grid cell
// Store the custom maps as noisemaps, and pseudo-procedurally generate them on load time

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap , BasicMesh , ColorShadedMesh , TexturedMesh
    }
    public DrawMode drawMode;

    public BasicColorData colorData;
    public ColorShaderData colorShaderData;
    public TextureData textureData;

    public Material[] materials;

    public int mapSize = 40;
    public bool autoUpdate;

    public int seed;
    public bool useFlatShading;
    public float noiseScale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public Vector2 offset;

    private void Start()
    {
        GenerateMap();
    }
    private void OnColorShaderDataUpdated()
    {
        colorShaderData.ApplyToMaterial( materials[ 0 ] );
    }
    private void OnTextureDataUpdated()
    {
        textureData.ApplyToMaterial( materials[ 1 ] );
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap( mapSize , seed , noiseScale , octaves , persistance , lacunarity , offset );
        MapDisplay mapDisplay = GetComponent<MapDisplay>();

        if ( drawMode == DrawMode.NoiseMap )
        {
            mapDisplay.DrawTexture( TextureGenerator.TextureFromHeightMap( noiseMap , mapSize ) );
        }
        else if ( drawMode == DrawMode.BasicMesh )
        {
            GenerateMapFromColorMap( mapDisplay , noiseMap );
        }
        else if ( drawMode == DrawMode.ColorShadedMesh )
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh( noiseMap , mapSize , meshHeightMultiplier , meshHeightCurve , useFlatShading );
            colorShaderData.UpdateMeshHeights( materials[ 0 ] , minHeight , maxHeight );
            mapDisplay.DrawMesh( meshData );
        }
        else if ( drawMode == DrawMode.TexturedMesh )
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh( noiseMap , mapSize , meshHeightMultiplier , meshHeightCurve , useFlatShading );
            textureData.UpdateMeshHeights( materials[ 1 ] , minHeight , maxHeight );
            mapDisplay.DrawMesh( meshData );
        }    
    }
    private void GenerateMapFromColorMap( MapDisplay mapDisplay , float[,] noiseMap )
    {
        Color[] colorMap = new Color[ mapSize * mapSize ];

        for ( int y = 0; y < mapSize; y++ )
        {
            for ( int x = 0; x < mapSize; x++ )
            {
                float currentHeight = noiseMap[ x , y ];
                for ( int i = 0; i < colorData.regions.Length; i++ )
                {
                    if ( currentHeight <= colorData.regions[ i ].height )
                    {
                        colorMap[ y * mapSize + x ] = colorData.regions[ i ].color;
                        break;
                    }
                }
            }
        }

        mapDisplay.DrawMeshBasic( MeshGenerator.GenerateTerrainMesh(
            noiseMap , mapSize , meshHeightMultiplier , meshHeightCurve , useFlatShading ) ,
            TextureGenerator.TextureFromColorMap( colorMap , mapSize ) );
    }

    private void OnValidate()
    {
        if ( mapSize < 1 )
            mapSize = 1;
        if ( lacunarity < 1 )
            lacunarity = 1;
        if ( octaves < 0 )
            octaves = 0;

        if ( drawMode == DrawMode.ColorShadedMesh )
        {
            GetComponent<MapDisplay>().UpdateMaterial( materials[ 0 ] );
            colorShaderData.OnValuesUpdated -= OnColorShaderDataUpdated;
            colorShaderData.OnValuesUpdated += OnColorShaderDataUpdated;
            GenerateMap();
        }
        else if ( drawMode == DrawMode.TexturedMesh )
        {
            GetComponent<MapDisplay>().UpdateMaterial( materials[ 1 ] );
            textureData.OnValuesUpdated -= OnTextureDataUpdated;
            textureData.OnValuesUpdated += OnTextureDataUpdated;
            GenerateMap();
        }
        else if ( drawMode == DrawMode.BasicMesh )
        {
            GetComponent<MapDisplay>().UpdateMaterial( materials[ 2 ] );
            GenerateMap();
        }

        if ( colorShaderData != null )
        {
            colorShaderData.OnValuesUpdated -= OnColorShaderDataUpdated;
            colorShaderData.OnValuesUpdated += OnColorShaderDataUpdated;
        }

        if ( textureData != null )
        {
            textureData.OnValuesUpdated -= OnTextureDataUpdated;
            textureData.OnValuesUpdated += OnTextureDataUpdated;
        }
    }

    private float minHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate( 0 );
        }
    }
    private float maxHeight
    {
        get
        {
            return meshHeightMultiplier * meshHeightCurve.Evaluate( 1 );
        }
    }
}
