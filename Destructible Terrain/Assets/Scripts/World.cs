using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// World is a composite of chunks. Inits them and redirects all operations to the chunks. Operates on a original texture coordinates to find a chunk.
    /// </summary>
    public class World : MonoBehaviour
    {
        [SerializeField]
        [Min(1)]
        private int chunksX=16;
        [SerializeField]
        [Min(1)]
        private int chunksY=16;

        [SerializeField]
        private Texture2D originalTexture=null;
        [Tooltip("PPU of the used texture.")]
        public int PPU = 32;

        [SerializeField]
        [Tooltip("GameObject representing chunk that will be spawned.")]
        private GameObject baseChunk=null;

        private int chunkSizeX;
        private int chunkSizeY;
        
        List<DestructibleTerrainChunk> chunks;
    
        // Start is called before the first frame update
        void Start()
        {
            CreateChunks();
            Camera.main.transform.position = new Vector3((float)originalTexture.width/PPU/2.0f, (float)originalTexture.height/PPU/2.0f, -10.0f);
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

        /// <summary>
        /// Deletes terrain using each range in the shape.
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="s">Shape to delete terrain with</param>
        public void DestroyTerrain(int x, int y, Shape s)
        {
            int k = 0;
            foreach (Range r in s.ranges)
            {
                DestroyTerrain(x+k,y,r, s.height);
                k++;
            }
        }

        public void DestroyTerrain(Vector2Int v, Shape s)
        {
            DestroyTerrain(v.x, v.y, s);
        }

        /// <summary>
        /// Creates a black outline in the world of given shape
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="s">Shape</param>
        public void MakeOutline(int x, int y, Shape s)
        {
            int k = 0;
            foreach (Range r in s.ranges)
            {
                for(int i = r.min;i<r.max;i++)
                    MakeOutline(x+k,y+i,s.outlineColor);
                k++;
            }
        }

        /// <summary>
        /// DestroyTerrain is used to delete a single range.
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="r">Range</param>
        /// <param name="height">Maximum height of an range (used in shape deletion - skip it)</param>
        /// <returns></returns>
        public bool DestroyTerrain(int x, int y, Range r, int height=-1)
        {
            if (height == -1)
                height = r.Length();

            int xchunk = (x + chunkSizeX / 2) / chunkSizeX;
            int ychunk = (y + chunkSizeY / 2) / chunkSizeY;
            int posInChunkX = x - xchunk * chunkSizeX + chunkSizeX / 2;
            int posInChunkY = y - ychunk * chunkSizeY + chunkSizeY / 2;
            int cid = xchunk * chunksY + ychunk;

            int k = 0;
            //Iterate over possible chunks vertically that can be contained in destruction for this range
            while (true)
            {
                if (cid >= 0 && cid < chunks.Count && k+ychunk<chunksY && (k-1)*chunkSizeY<=height)
                {
                    if(chunks[cid].DestroyTerrain(posInChunkX, posInChunkY - k * chunkSizeY, r))
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

        /// <summary>
        /// Calls an DestroyTerrain for a chunk
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <returns>True if any changes were made</returns>
        public bool DestroyTerrain(int x, int y)
        {
            int xchunk = (x+chunkSizeX/2)/chunkSizeX;
            int ychunk = (y+chunkSizeY/2)/chunkSizeY;
            int posInChunkX = x-xchunk*chunkSizeX + chunkSizeX/2;
            int posInChunkY = y-ychunk*chunkSizeY + chunkSizeY/2;
            int cid = xchunk*chunksY + ychunk;
            if(cid>=0 && cid<chunks.Count)
            {
                return chunks[cid].DestroyTerrain(posInChunkX,posInChunkY);
            }
            return false;
        }


        public bool DestroyTerrain(Vector2Int v)
        {
            return DestroyTerrain(v.x, v.y);
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
                chunks[cid].MakeOutline(posInChunkX,posInChunkY,outlineCol);
            }
        
        }

        /// <summary>
        /// Checks if there is an terrain or air in given pixel
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <returns>True - filled, False - not filled</returns>
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


        /// <param name="x">X position at Texture2D</param>
        /// <param name="y">Y position at Texture2D</param>
        /// <returns>Color at (x,y) for an original texture.</returns>
        public Color ColorAt(int x, int y)
        {
            int xchunk = (x + chunkSizeX / 2) / chunkSizeX;
            int ychunk = (y + chunkSizeY / 2) / chunkSizeY;
            int posInChunkX = x - xchunk * chunkSizeX + chunkSizeX / 2;
            int posInChunkY = y - ychunk * chunkSizeY + chunkSizeY / 2;
            int cid = xchunk * chunksY + ychunk;
            if (cid >= 0 && cid < chunks.Count)
            {
                return chunks[cid].ColorAt(posInChunkX, posInChunkY);
            }
            return new Color(0,0,0,0);
        }

        /// <summary>
        /// Given a position on the scene, returns a position in the World. World = this world class, not a position in a Unity World.
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