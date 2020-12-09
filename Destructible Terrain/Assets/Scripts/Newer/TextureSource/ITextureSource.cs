using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public interface ITextureSource
    {
        Texture2D Texture { get; set; }
        int PPU { get; set; }

        void SetUpToRenderer(SpriteRenderer spriteRenderer);
    }
}
