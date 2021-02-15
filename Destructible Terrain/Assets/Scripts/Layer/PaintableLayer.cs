using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class PaintableLayer<T> : MonoBehaviour, ILayer<T> where T:PaintableChunk
    {

        [field:SerializeField]
        public int ChunkCountX { get; set; }
        [field: SerializeField]
        public int ChunkCountY { get; set; }

        protected int chunkSizeX;
        protected int chunkSizeY;

        [SerializeField]
        protected GameObject chunkTemplate;

        [field:SerializeField]
        public Texture2D OriginalTexture { get; set; }

        [field:SerializeField]
        public int PPU { get; protected set; }

        [SerializeField]
        protected FilterMode filterMode;

        [field: SerializeField]
        public int SortingLayerID { get; set; }

        public List<T> Chunks { get; private set; }



        /// <summary>
        /// Spawns all chunks that are required for full functionality and adds them to the list named 'Chunks'.
        /// </summary>
        public virtual void SpawnChunks()
        {

            Chunks = new List<T>();
            chunkSizeX = OriginalTexture.width / ChunkCountX;
            chunkSizeY = OriginalTexture.height / ChunkCountY;

            for (int i = 0; i < ChunkCountX; i++)
            {
                for (int j = 0; j < ChunkCountY; j++)
                {
                    Texture2D piece = new Texture2D(chunkSizeX, chunkSizeY);
                    piece.filterMode = filterMode; 
                    piece.SetPixels(0, 0, chunkSizeX, chunkSizeY, OriginalTexture.GetPixels(i * chunkSizeX, j * chunkSizeY, chunkSizeX, chunkSizeY));
                    piece.Apply();


                    GameObject c = Instantiate(chunkTemplate);

                    c.name = $"Chunk{i * ChunkCountY + j}";

                    PaintableChunk pc = c.GetComponent<PaintableChunk>();
                    if (pc == null)
                        pc = c.AddComponent<PaintableChunk>();

                    pc.TextureSource = c.GetComponent<ITextureSource>();
                    if (pc.TextureSource == null)
                        pc.TextureSource = c.AddComponent<SingleTextureSource>();

                    pc.TextureSource.Texture = piece;
                    pc.TextureSource.PPU = PPU;

                    SpriteRenderer sr = c.GetComponent<SpriteRenderer>();
                    if(sr==null)
                        sr=c.AddComponent<SpriteRenderer>();

                    c.transform.position = transform.position + new Vector3(i * (float)chunkSizeX / PPU, j * (float)chunkSizeY / PPU, 0);
                    c.transform.SetParent(transform);

                    Chunks.Add(c.GetComponent<T>());
                }
            }
        }

        public virtual void InitChunks()
        {
            foreach(PaintableChunk t in Chunks)
            {
                t.SortingLayerID = SortingLayerID;
                t.Init();
            }
        }

        public int GetChunkIDByPosition(Vector2Int position)
        {
            int xchunk = position.x / chunkSizeX;
            int ychunk = position.y / chunkSizeY;
            int cid = xchunk * ChunkCountY + ychunk;
            return cid;
        }

        public PaintableChunk GetChunkByPosition(Vector2Int position)
        {
            return Chunks[GetChunkIDByPosition(position)];
        }

        public void Paint(PaintingParameters paintingParameters)
        {
            int k = 0;
            foreach (Range r in paintingParameters.Shape.Ranges)
            {
                PaintColumn(paintingParameters.Position.x + k, paintingParameters.Position.y+r.Min, r, paintingParameters);
                k++;
            }
        }

        /// <summary>
        /// Painting a single column (using a range).
        /// </summary>
        /// <param name="x">Global position X</param>
        /// <param name="y">Global position Y</param>
        /// <param name="r">Range from a shape</param>
        /// <param name="c">Color to be painted</param>
        private void PaintColumn(int x, int y, Range r, PaintingParameters pp)
        {
            
            int height = r.Length;
            //We don't use a method for getting chunk id because we need some variables
            int xchunk = x / chunkSizeX;
            int ychunk = y / chunkSizeY;
            int posInChunkX = x - xchunk * chunkSizeX;
            int posInChunkY = y - ychunk * chunkSizeY;
            int cid = xchunk * ChunkCountY + ychunk;

            int k = 0;
            //Iterate over possible chunks vertically that can be contained in painting for this range
            while (true)
            {
                
                if (cid >= 0 && cid < Chunks.Count && k + ychunk < ChunkCountY && (k - 1) * chunkSizeY <= height)
                {
                    Chunks[cid].Paint(new RectInt(posInChunkX, posInChunkY - k * chunkSizeY, 1, r.Length+1), pp);
                    cid++;
                    k++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
