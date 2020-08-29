﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// World is a composite of chunks. Inits them and redirects all operations to the chunks.
    /// </summary>
    public class World : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int chunksX;
        [SerializeField]
        [Min(1)]
        private int chunksY;

        [SerializeField]
        private Texture2D originalTexture;
        [Tooltip("PPU of the used texture.")]
        public int PPU = 32;

        [SerializeField]
        [Tooltip("GameObject representing chunk that will be spawned.")]
        private GameObject baseChunk;

        private int chunkSizeX;
        private int chunkSizeY;
        
        List<DestructibleTerrainChunk> chunks;
    
        // Start is called before the first frame update
        void Start()
        {
            CreateChunks();
            Camera.main.transform.position = new Vector3((float)originalTexture.width/PPU/2.0f, (float)originalTexture.height/PPU/2.0f, -10.0f);
        }

        void Update()
        {
        }

        /// <summary>
        /// Inits all chunks. Splits textures for them and makes sure they get DestructibleTerrainChunk.
        /// </summary>
        void CreateChunks()
        {
            chunks = new List<DestructibleTerrainChunk>();
            Texture2D[] pieces = new Texture2D[chunksX*chunksY];
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
                    pieces[i*chunksY + j] = piece;

                    GameObject c = Instantiate(baseChunk);
                    c.transform.position = gameObject.transform.position+new Vector3(i*(float)chunkSizeX/PPU,j*(float)chunkSizeY/PPU,0);
                    c.GetComponent<SpriteRenderer>().sprite = Sprite.Create(piece,new Rect(0,0,chunkSizeX,chunkSizeY),new Vector2(0.5f,0.5f),PPU);
                    c.transform.SetParent(transform);

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

        public void DestroyTerrainWithShape(Vector2Int v, Shape s, float power)
        {
            DestroyTerrainWithShape(v.x, v.y, s, power);
        }

        public void DestroyTerrainWithShapeOld(int x, int y, Shape s, float power)
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

        public void DestroyTerrainWithShape(int x, int y, Shape s, float power)
        {
            int k = 0;
            foreach (Range r in s.columns)
            {
                DestroyTerrainWithRange(x+k,y,r, s.height);
                k++;
            }
        }

        public bool DestroyTerrain(Vector2Int v, float power)
        {
            return DestroyTerrain(v.x, v.y, power);
        }

        public bool DestroyTerrainWithRange(int x, int y, Range r, int height)
        {

            int xchunk = (x + chunkSizeX / 2) / chunkSizeX;
            int ychunk = (y + chunkSizeY / 2) / chunkSizeY;
            int posInChunkX = x - xchunk * chunkSizeX + chunkSizeX / 2;
            int posInChunkY = y - ychunk * chunkSizeY + chunkSizeY / 2;
            int cid = xchunk * chunksY + ychunk;
            int k = 0;
            while (true)
            {
                if (cid >= 0 && cid < chunks.Count && k+ychunk<chunksY && (k-1)*chunkSizeY<=height)
                {
                    if(chunks[cid].DestroyTerrainWithRange(posInChunkX, posInChunkY - k * chunkSizeY, r))
                        chunks[cid].updateTerrainOnNextFrame = true;

                    cid++;
                    k++;
                }
                else
                {
                    break;
                }
            }
            

            return false;

        }

        public bool DestroyTerrain(int x, int y, float power)
        {
            int xchunk = (x+chunkSizeX/2)/chunkSizeX;
            int ychunk = (y+chunkSizeY/2)/chunkSizeY;
            int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
            int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
            int cid = xchunk*chunksY + ychunk;
            if(cid>=0 && cid<chunks.Count)
            {
                chunks[cid].updateTerrainOnNextFrame=true;
                return chunks[cid].DestroyTerrain(posInChunkX,posInChunkY,power);
            }
            return false;
        }

        public void MakeOutline(int x, int y, Color outlineCol)
        {
            int xchunk = (x+chunkSizeX/2)/chunkSizeX;
            int ychunk = (y+chunkSizeY/2)/chunkSizeY;
            int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
            int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
            int cid = xchunk*chunksY + ychunk;
            if(cid>=0 && cid<chunks.Count)
            {
                chunks[cid].updateTerrainOnNextFrame=true;
                chunks[cid].MakeOutline(posInChunkX,posInChunkY,Color.black);
            }
        
        }

        public bool FilledAt(int x, int y)
        {
            int xchunk = (x+chunkSizeX/2)/chunkSizeX;
            int ychunk = (y+chunkSizeY/2)/chunkSizeY;
            int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
            int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
            int cid = xchunk*chunksY + ychunk;
            if(cid>=0 && cid<chunks.Count)
            {
                return chunks[cid].FilledAt(posInChunkX, posInChunkY);
            }
            return false;
        }

        /// <summary>
        /// Given a position on the scene, returns a position in the World.
        /// </summary>
        /// <param name="scenePos">Position in scene. Remember to make World offset (0,0).</param>
        /// <returns></returns>
        public Vector2Int ScenePositionToWorldPosition(Vector2 scenePos)
        {
            return new Vector2Int(SceneCoorinateToWorldCoordinate(scenePos.x), SceneCoorinateToWorldCoordinate(scenePos.y));
        }

        public int SceneCoorinateToWorldCoordinate(float coord)
        {
            return (int)(coord * PPU);
        }

    }

}