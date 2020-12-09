using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Basic SingleTextureSource that has only one texture and keeps a copy of starting state of that texture.
    /// </summary>
    public class BlankSingleTextureSource: SingleTextureSource
    {
        [SerializeField]
        private Color startingColor;

        private Texture2D texture;
        public new Texture2D Texture
        {
            get => texture;
            set
            {
                base.Texture = value;

                Texture = new Texture2D(Texture.width, Texture.height);

                Color[] colors = new Color[Texture.width * Texture.height];
                colors = colors.Select(c => startingColor).ToArray();

                Texture.SetPixels(colors);
                Texture.Apply();

                Graphics.CopyTexture(Texture, OriginalTexture);
                OriginalTexture.Apply();
            }
        }

    }
}
