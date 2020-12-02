using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class BasicPaintableLayer : PaintableLayer<PaintableChunk, SingleTextureSource>
    {
        public void Start()
        {
            InitChunks();
            Camera.main.transform.position = new Vector3((float)originalTexture.width / PPU / 2.0f, (float)originalTexture.height / PPU / 2.0f, -10.0f);

            
        }

        public void Update()
        {
            if(Input.GetMouseButton(0))
            {
                Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                Paint(new PaintingParameters() { Color = Color.clear, Position = new Vector2Int((int)(p.x*PPU)-16, (int)(p.y*PPU)-16), Shape = Shape.GenerateShapeCircle(16) });
            }
        }
    }
}
