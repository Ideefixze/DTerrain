using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Basic SingleTextureSource that has only one texture and keeps a copy of starting state of that texture.
    /// </summary>
    public class GeneratedTextureSource: MonoBehaviour, ITextureSource
    {
        public ITextureGenerator TextureGenerator { get; set; }
        public Texture2D Texture { get => TextureGenerator.GenerateTexture(); set { } }
        public int PPU { get; set; }

        public void SetUpToRenderer(SpriteRenderer spriteRenderer)
        {
            
        }
    }
}
