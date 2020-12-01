using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Basic SingleTextureSource that has only one texture and keeps a copy of starting state of that texture.
    /// </summary>
    public class SingleTextureSource: ITextureSource
    {
        public Texture2D OriginalTexture { get; private set; }
        public Texture2D Texture { get; set; }

        public SingleTextureSource(Texture2D startingTexture)
        {
            Graphics.CopyTexture(startingTexture, OriginalTexture);
            Graphics.CopyTexture(startingTexture, Texture);
        }
    }
}
