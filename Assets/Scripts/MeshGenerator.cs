using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh( float[,] heightMap , int mapSize , float heightMultiplier , AnimationCurve meshHeightCurve , bool useFlatShading )
    {
        MeshData meshData = new MeshData( mapSize , useFlatShading );
        int vertexIndex = 0;

        for ( int y = 0; y < mapSize; y++ )
        {
            for ( int x = 0; x < mapSize; x++ )
            {
                float height = meshHeightCurve.Evaluate( heightMap[ x , y ] ) * heightMultiplier;
                meshData.vertices[ vertexIndex ] = new Vector3( x , height , y );
                meshData.uvs[ vertexIndex ] = new Vector2( x / ( float ) mapSize , y / ( float ) mapSize );

                if ( x < mapSize - 1 && y < mapSize - 1 )
                {
                    meshData.AddTriangle( vertexIndex , vertexIndex + mapSize + 1 , vertexIndex + mapSize );
                    meshData.AddTriangle( vertexIndex + mapSize + 1 , vertexIndex , vertexIndex + 1 );
                }

                vertexIndex++;
            }
        }

        meshData.ProcessMesh();

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    private int triangleIndex;
    private bool useFlatShading;

    public MeshData( int mapSize , bool _useFlatShading )
    {
        vertices = new Vector3[ mapSize * mapSize ];
        triangles = new int[ ( mapSize - 1 ) * ( mapSize - 1 ) * 6 ];
        uvs = new Vector2[ mapSize * mapSize ];

        triangleIndex = 0;
        useFlatShading = _useFlatShading;
    }

    public void AddTriangle( int a , int b , int c )
    {
        triangles[ triangleIndex + 0 ] = c;
        triangles[ triangleIndex + 1 ] = b;
        triangles[ triangleIndex + 2 ] = a;

        triangleIndex += 3;
    }

    public void ProcessMesh()
    {
        if ( useFlatShading )
            FlatShading();
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        //mesh.normals = CalculateNormals();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[ vertices.Length ];
        int triangleCount = triangles.Length / 3;

        for ( int i = 0; i < triangleCount; i++ )
        {
            int normalTriangleIndex = i * 3;

            int vertIndexA = triangles[ normalTriangleIndex + 0 ];
            int vertIndexB = triangles[ normalTriangleIndex + 1 ];
            int vertIndexC = triangles[ normalTriangleIndex + 2 ];

            Vector3 triangleNormal = SurfaceNormalFromIndices( vertIndexA , vertIndexB , vertIndexC );

            vertexNormals[ vertIndexA ] += triangleNormal;
            vertexNormals[ vertIndexB ] += triangleNormal;
            vertexNormals[ vertIndexC ] += triangleNormal;
        }

        for ( int i = 0; i < vertexNormals.Length; i++ )
            vertexNormals[ i ].Normalize();

        return vertexNormals;
    }

    private Vector3 SurfaceNormalFromIndices( int indexA , int indexB , int indexC )
    {
        Vector3 pointA = vertices[ indexA ];
        Vector3 pointB = vertices[ indexB ];
        Vector3 pointC = vertices[ indexC ];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross( sideAB , sideAC ).normalized;
    }

    private void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[ triangles.Length ];
        Vector2[] flatShadedUvs = new Vector2[ triangles.Length ];

        for ( int i = 0; i < triangles.Length; i++ )
        {
            flatShadedVertices[ i ] = vertices[ triangles[ i ] ];
            flatShadedUvs[ i ] = uvs[ triangles[ i ] ];
            triangles[ i ] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }
}