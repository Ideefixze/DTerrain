using System;
using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace DTerrain
{
    public class SimulatedLayer : PaintableLayer<SimulatedChunk>
    {
        [Help("Size of a SimulatedLayer and OriginalTexture is automatically set before chunk spawning to fit the parent layer. No OriginalTexture will create empty texture with color (0,0,0,0).")]
        public BasicPaintableLayer ParentLayer;
        protected bool parentSet = false;

        public override void SpawnChunks()
        {
            ChunkCountX = ParentLayer.ChunkCountX;
            ChunkCountY = ParentLayer.ChunkCountY;

            if(OriginalTexture!=null)
            {
                OriginalTexture.Resize(ParentLayer.OriginalTexture.width, ParentLayer.OriginalTexture.height);
                OriginalTexture.Apply();
            }
            else
            {
                OriginalTexture = new Texture2D(ParentLayer.OriginalTexture.width, ParentLayer.OriginalTexture.height);
                Color[] c = new Color[ParentLayer.OriginalTexture.width * ParentLayer.OriginalTexture.height];
                c = c.Select(color => new Color(0, 0, 0, 0)).ToArray();
                OriginalTexture.SetPixels(c);
            }

            base.SpawnChunks();
        }

        public override void InitChunks()
        {
            base.InitChunks();

            int k = 0;
            foreach (PaintableChunk pc in ParentLayer.Chunks)
            {
                Chunks[k].Parent = pc;

                //If it's complex chunk, add a child
                ComplexChunk cc = (ComplexChunk)pc;
                if(cc!=null)cc.ChildChunks.Add(Chunks[k]);
                
                k++;


            }
            parentSet = true;
        }

        IEnumerator WaitForParentInit()
        {
            while(ParentLayer.Chunks==null)
                yield return new WaitForEndOfFrame();

            yield return null;
        }
    }
}
