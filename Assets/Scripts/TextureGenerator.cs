using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap( Color[] colorMap , int mapSize )
    {
        Texture2D texture = new Texture2D( mapSize , mapSize );
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels( colorMap );
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap( float[,] heightMap , int mapSize )
    {
        Color[] colorMap = new Color[ mapSize * mapSize ];

        for ( int y = 0; y < mapSize; y++ )
        {
            for ( int x = 0; x < mapSize; x++ )
                colorMap[ y * mapSize + x ] = Color.Lerp( Color.black , Color.white , heightMap[ x , y ] );
        }

        return TextureFromColorMap( colorMap , mapSize );
    }
}
