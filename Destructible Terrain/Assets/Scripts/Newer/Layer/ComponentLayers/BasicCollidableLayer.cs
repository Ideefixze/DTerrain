using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

namespace DTerrain
{


    public class BasicCollidableLayer : PaintableLayer<CollidableChunk, SingleTextureSource>
    {
        protected enum ColliderAlgorithmType
        {
            QUADTREE,
            NONE
        }

        protected ColliderAlgorithmType colliderAlgorithm;
        public  void Start()
        {
            SpawnChunks();
            ColliderInit();
            InitChunks();
        }

        protected virtual void ColliderInit()
        {
            foreach (CollidableChunk cc in Chunks)
            {
                if (colliderAlgorithm == ColliderAlgorithmType.QUADTREE)
                    cc.gameObject.AddComponent<QuadTreeChunkCollider>();
            }
        }
    }
}
