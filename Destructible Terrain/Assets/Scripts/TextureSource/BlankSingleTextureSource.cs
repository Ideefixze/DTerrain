using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Blank Texture that will be filled with a color whenever you set any texture to it. (Remains only size of set texture, but not it's color data).
    /// </summary>
    public class BlankSingleTextureSource: SingleTextureSource
    {
        [SerializeField]
        private Color startingColor;

        private Texture2D texture;
        public Texture2D Texture
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
