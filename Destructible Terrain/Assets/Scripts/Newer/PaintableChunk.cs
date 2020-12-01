using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class PaintableChunk : IChunk
    {
        public ITextureSource TextureSource { get; }

        public void Paint(RectInt r, Color c)
        {
            int len = r.width * r.height;
            Color[] cs = new Color[len];
            for (int i = 0; i < len; i++)
                cs[i] = c;
            TextureSource.Texture.SetPixels(r.x, r.y, r.width, r.height, cs);
        }
    }
}

