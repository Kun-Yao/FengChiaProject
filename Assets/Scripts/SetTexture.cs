using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTexture : MonoBehaviour
{

    public int TextureAtlasSizeInBlocks = 4;
    public BlockType blockType;

    private float NormalizedBlockTextureSize
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    Vector2[] uvs = new Vector2[24];

    void Awake()
    {

        if (this.tag == "Cylinder")
        {

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = new Vector2[vertices.Length];

            float y = 16 / TextureAtlasSizeInBlocks;
            float x = 16 / (y * TextureAtlasSizeInBlocks);
            x *= NormalizedBlockTextureSize;
            y *= NormalizedBlockTextureSize;
            y = 1f - y - NormalizedBlockTextureSize;

            for (var i = 0; i < vertices.Length / 4; i++)
            {

                uvs[i] = new Vector2(x, y);
                uvs[i + 1] = new Vector2(x + NormalizedBlockTextureSize, y);
                uvs[i + 2] = new Vector2(x, y + NormalizedBlockTextureSize);
                uvs[i + 3] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
            }
            this.GetComponent<MeshFilter>().mesh.uv = uvs;
        }
        else
        {
            Front(blockType.GetTextureID(0));
            Top(blockType.GetTextureID(1));
            Back(blockType.GetTextureID(2));
            Bottom(blockType.GetTextureID(3));
            Left(blockType.GetTextureID(4));
            Right(blockType.GetTextureID(5));

            this.GetComponent<MeshFilter>().mesh.uv = uvs;
        }
    }

    void Front(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[0] = new Vector2(x, y);
        uvs[1] = new Vector2(x + NormalizedBlockTextureSize, y);
        uvs[2] = new Vector2(x, y + NormalizedBlockTextureSize);
        uvs[3] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
    }
    void Top(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[4] = new Vector2(x, y + NormalizedBlockTextureSize);
        uvs[5] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
        uvs[8] = new Vector2(x, y);
        uvs[9] = new Vector2(x + NormalizedBlockTextureSize, y);
    }
    void Back(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[6] = new Vector2(x + NormalizedBlockTextureSize, y);
        uvs[7] = new Vector2(x, y);
        uvs[10] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
        uvs[11] = new Vector2(x, y + NormalizedBlockTextureSize);
    }
    void Bottom(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[12] = new Vector2(x, y);
        uvs[13] = new Vector2(x, y + NormalizedBlockTextureSize);
        uvs[14] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
        uvs[15] = new Vector2(x + NormalizedBlockTextureSize, y);
    }
    void Left(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[16] = new Vector2(x, y);
        uvs[17] = new Vector2(x, y + NormalizedBlockTextureSize);
        uvs[18] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
        uvs[19] = new Vector2(x + NormalizedBlockTextureSize, y);
    }
    void Right(int textureID)
    {

        float y = textureID / TextureAtlasSizeInBlocks;
        float x = textureID - (y * TextureAtlasSizeInBlocks);
        x *= NormalizedBlockTextureSize;
        y *= NormalizedBlockTextureSize;
        y = 1f - y - NormalizedBlockTextureSize;

        uvs[20] = new Vector2(x, y);
        uvs[21] = new Vector2(x, y + NormalizedBlockTextureSize);
        uvs[22] = new Vector2(x + NormalizedBlockTextureSize, y + NormalizedBlockTextureSize);
        uvs[23] = new Vector2(x + NormalizedBlockTextureSize, y);
    }

    public Vector2[] addTexture()
    {
        return uvs;
    }
}

[System.Serializable]
public class BlockType
{

    [Header("Texture Values")]
    public int frontFaceTexture;
    public int topFaceTexture;
    public int backFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    public int GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
                return frontFaceTexture;
            case 1:
                return topFaceTexture;
            case 2:
                return backFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;
        }
    }
}
