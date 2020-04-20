using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int chunksX;
    public int chunksY;

    public int chunkSizeX;
    public int chunkSizeY;
    public Texture2D originalTexture;
    List<DestructibleTerrainChunk> chunks;

    public GameObject BaseChunk;

    public int PPU=32;
    Shape destructionShape;
    
    // Start is called before the first frame update
    void Start()
    {
        destructionShape = Shape.GenerateShapeCircle(8);
        CreateChunks();
        Camera.main.transform.position = new Vector3((float)originalTexture.width/PPU/2.0f, (float)originalTexture.height/PPU/2.0f, -10.0f);
    }

    void Update()
    {
        MouseDestruction();
    }

    void CreateChunks()
    {
        chunks = new List<DestructibleTerrainChunk>();
        Texture2D[] piecies = new Texture2D[chunksX*chunksY];
        chunkSizeX = originalTexture.width/chunksX;
        chunkSizeY = originalTexture.height/chunksY;

        for(int i = 0; i<chunksX;i++)
        {
            for(int j = 0; j<chunksY;j++)
            {
                Texture2D piece = new Texture2D(chunkSizeX, chunkSizeY);
                piece.filterMode = FilterMode.Point;
                piece.SetPixels(0,0,chunkSizeX,chunkSizeY, originalTexture.GetPixels(i*chunkSizeX,j*chunkSizeY,chunkSizeX,chunkSizeY));
                piece.Apply();
                piecies[i*chunksY + j] = piece;

                GameObject c = Instantiate(BaseChunk);
                c.transform.position = gameObject.transform.position+new Vector3(i*(float)chunkSizeX/PPU,j*(float)chunkSizeY/PPU,0);
                c.GetComponent<SpriteRenderer>().sprite = Sprite.Create(piece,new Rect(0,0,chunkSizeX,chunkSizeY),new Vector2(0.5f,0.5f),PPU);
                
                DestructibleTerrainChunk chunkComp = c.GetComponent<DestructibleTerrainChunk>();
                
                if(chunkComp!=null)
                    chunks.Add(chunkComp);
                else
                {
                    Debug.LogError("Chunk has no DestructibleTerrainChunk component!");
                    return;
                }
            }
        }
    }

    public void MouseDestruction()
    {
        if(Input.GetMouseButton(0))
        {
            Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int xPos = (int)(mPos.x*PPU);
            int yPos = (int)(mPos.y*PPU);
            int r = 8;
            DestroyTerrainWithShape(xPos - r,yPos - r, destructionShape,10f);
        }
    }

    public void DestroyTerrainWithShape(int x, int y, Shape s, float power)
    {
        int k = 0;
        foreach(Range r in s.columns)
        {
            MakeOutline(x+k,y+r.min,Color.black);
            for(int i = r.min+1; i<r.max-1;i++)
            {
                if(k>0 && k<s.columns.Count-1)
                    DestroyTerrain(x+k,y+i,power);
                else
                    MakeOutline(x+k,y+i,Color.black);

            }
            MakeOutline(x+k,y+r.max-1,Color.black);
            k++;
        }
    }

    public bool DestroyTerrain(int x, int y, float power)
    {
        int xchunk = (x+chunkSizeX/2)/chunkSizeX;
        int ychunk = (y+chunkSizeY/2)/chunkSizeY;
        int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
        int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
        chunks[xchunk*chunksY + ychunk].updateTerrainOnNextFrame=true;
        return chunks[xchunk*chunksY + ychunk].DestroyTerrain(posInChunkX,posInChunkY,power);
    }

    public void MakeOutline(int x, int y, Color outlineCol)
    {
    int xchunk = (x+chunkSizeX/2)/chunkSizeX;
        int ychunk = (y+chunkSizeY/2)/chunkSizeY;
        int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
        int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
        chunks[xchunk*chunksY + ychunk].updateTerrainOnNextFrame=true;
        chunks[xchunk*chunksY + ychunk].MakeOutline(posInChunkX,posInChunkY,Color.black);
    }

    // Update is called once per frame

}
