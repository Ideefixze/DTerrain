using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

namespace DTerrain
{
    public enum ColliderAlgorithmType
    {
        QUADTREE,
        NONE
    }

    public class BasicCollidableLayer : PaintableLayer<CollidableChunk, SingleTextureSource>
    {
        public ColliderAlgorithmType colliderAlgorithm;
        public void Start()
        {
            SpawnChunks();

            foreach(CollidableChunk cc in chunks)
            {
                if(colliderAlgorithm==ColliderAlgorithmType.QUADTREE)
                    cc.gameObject.AddComponent<QuadTreeChunkCollider>();
            }

            InitChunks();
            
        }

        public void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                Paint(new PaintingParameters() { Color = Color.clear, Position = new Vector2Int((int)(p.x * PPU) - 16, (int)(p.y * PPU) - 16), Shape = Shape.GenerateShapeCircle(16) });
            }
        }
    }
}
