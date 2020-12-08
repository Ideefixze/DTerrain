using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class BlankPaintableLayer : BasicPaintableLayer
    {
        public override void Start()
        {
            base.Start();
            MakeChunksBlank();
        }

        private void MakeChunksBlank()
        {
            foreach(PaintableChunk pc in Chunks)
            {
                pc.TextureSource.Texture = new Texture2D(pc.TextureSource.Texture.width, pc.TextureSource.Texture.height);

                Color[] colors = new Color[pc.TextureSource.Texture.width * pc.TextureSource.Texture.height];
                colors = colors.Select(c => new Color(0, 0, 0, 0)).ToArray();

                pc.TextureSource.Texture.SetPixels(colors);
                pc.TextureSource.Texture.Apply();
                pc.Init();
            }

        }

    }
}
