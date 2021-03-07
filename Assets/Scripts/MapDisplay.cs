using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public void DrawTexture( Texture2D texture )
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3( texture.width , 1 , texture.height );
    }

    public void DrawMeshBasic( MeshData meshData , Texture2D texture )
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawMesh( MeshData meshData )
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
    }

    public void UpdateMaterial( Material material )
    {
        meshRenderer.sharedMaterial = material; 
    }
}
