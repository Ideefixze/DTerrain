using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class PaintableLayer<T, TSource> : MonoBehaviour, ILayer<T> where T:PaintableChunk where TSource:ITextureSource, new()
    {
        [field:SerializeField]
        public int ChunkCountX { get; set; }
        [field: SerializeField]
        public int ChunkCountY { get; set; }

        protected int chunkSizeX;
        protected int chunkSizeY;

        [SerializeField]
        protected Texture2D originalTexture = null;

        [SerializeField]
        protected int PPU;

        [SerializeField]
        protected FilterMode filterMode;

        protected List<T> chunks;

        public void SpawnChunks()
        {
            chunks = new List<T>();
            chunkSizeX = originalTexture.width / ChunkCountX;
            chunkSizeY = originalTexture.height / ChunkCountY;

            for (int i = 0; i < ChunkCountX; i++)
            {
                for (int j = 0; j < ChunkCountY; j++)
                {
                    Texture2D piece = new Texture2D(chunkSizeX, chunkSizeY);
                    piece.filterMode = filterMode; 
                    piece.SetPixels(0, 0, chunkSizeX, chunkSizeY, originalTexture.GetPixels(i * chunkSizeX, j * chunkSizeY, chunkSizeX, chunkSizeY));
                    piece.Apply();

                    GameObject c = new GameObject();

                    c.name = $"Chunk{i * ChunkCountY + j}";

                    PaintableChunk pc = c.AddComponent<T>();
                    pc.TextureSource = new TSource();
                    pc.TextureSource.Texture = piece;
                    pc.TextureSource.PPU = PPU;

                    c.AddComponent<SpriteRenderer>();
                    c.transform.position = gameObject.transform.position + new Vector3(i * (float)chunkSizeX / PPU, j * (float)chunkSizeY / PPU, 0);
                    c.transform.SetParent(transform);

                    chunks.Add(c.GetComponent<T>());
                }
            }
        }

        public void InitChunks()
        {
            foreach(T t in chunks)
            {
                t.Init();
            }
        }

        public void Paint(PaintingParameters paintingParameters)
        {
            int k = 0;
            foreach (Range r in paintingParameters.Shape.Ranges)
            {
                PaintColumn(paintingParameters.Position.x + k, paintingParameters.Position.y, r, paintingParameters.Color);
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
        private void PaintColumn(int x, int y, Range r, Color c)
        {
            int height = r.Length;

            int xchunk = (x + chunkSizeX / 2) / chunkSizeX;
            int ychunk = (y + chunkSizeY / 2) / chunkSizeY;
            int posInChunkX = x - xchunk * chunkSizeX + chunkSizeX / 2;
            int posInChunkY = y - ychunk * chunkSizeY + chunkSizeY / 2;
            int cid = xchunk * ChunkCountY + ychunk;

            int k = 0;
            //Iterate over possible chunks vertically that can be contained in destruction for this range
            while (true)
            {
                if (cid >= 0 && cid < chunks.Count && k + ychunk < ChunkCountY && (k - 1) * chunkSizeY <= height)
                {

                    chunks[cid].Paint(new RectInt(posInChunkX, posInChunkY - k * chunkSizeY + r.Min, 1, r.Length), c);
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
