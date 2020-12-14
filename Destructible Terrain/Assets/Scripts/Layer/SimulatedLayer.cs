using System;
using System.Linq;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DTerrain
{
    public class SimulatedLayer : PaintableLayer<SimulatedChunk>
    {
        [Help("Size of a SimulatedLayer is automatiaclly set and OriginalTexture is resize before chunk spawning to fit the parent layer. No OriginalTexture will create empty texture with color (0,0,0,0).")]
        public BasicPaintableLayer ParentLayer;
        protected bool parentSet = false;

        public override void SpawnChunks()
        {
            ChunkCountX = ParentLayer.ChunkCountX;
            ChunkCountY = ParentLayer.ChunkCountY;

            if(OriginalTexture!=null)
            {
                
                Texture2D tempTexture = new Texture2D(ParentLayer.OriginalTexture.width, ParentLayer.OriginalTexture.height);
                tempTexture.SetPixels(0, 0, Mathf.Min(tempTexture.width, OriginalTexture.width), Mathf.Min(tempTexture.height, OriginalTexture.height), 
                    OriginalTexture.GetPixels(0, 0, Mathf.Min(tempTexture.width,OriginalTexture.width), Mathf.Min(tempTexture.height, OriginalTexture.height)));
                tempTexture.Apply();
                OriginalTexture = tempTexture;
                
            }
            else
            {
                OriginalTexture = new Texture2D(ParentLayer.OriginalTexture.width, ParentLayer.OriginalTexture.height);
                Color[] c = new Color[ParentLayer.OriginalTexture.width * ParentLayer.OriginalTexture.height];
                c = c.Select(color => new Color(0, 0, 0, 0)).ToArray();
                OriginalTexture.SetPixels(c);
                OriginalTexture.Apply();
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
                try
                {
                    ComplexChunk cc = (ComplexChunk)pc;
                    if (cc != null) cc.ChildChunks.Add(Chunks[k]);
                }
                catch
                {

                }
                
                k++;
            }
            parentSet = true;
        }



        public void Simulate()
        {
            if (parentSet == false) return;

            foreach(SimulatedChunk sc in Chunks)
            {
                sc.Simulate();
            }
        }
    }
}
