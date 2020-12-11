using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PaintableChunk : MonoBehaviour, ITexturedChunk
    {
        public SpriteRenderer SpriteRenderer { get; private set; }
        public ITextureSource TextureSource { get; set; }
        protected bool painted=false;


        public virtual void Init()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.sprite = Sprite.Create(TextureSource.Texture, new Rect(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), new Vector2(0.5f, 0.5f), TextureSource.PPU);
            TextureSource.SetUpToRenderer(SpriteRenderer);

        }

        public virtual void Paint(RectInt r, PaintingParameters pp)
        {
            //Find common rect that will be applied on this texture rect
            RectInt common;
            r.Intersects(new RectInt(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), out common);

            //Generate color array...
            int len = common.width * common.height;
            Color[] cs = new Color[len];

            //...using paiting method
            if (pp.PaintingMode == PaintingMode.REPLACE_COLOR)
                for (int i = 0; i < len; i++)
                    cs[i] = pp.Color;
            else if (pp.PaintingMode == PaintingMode.ADD_COLOR)
            {
                for (int i = 0; i < common.width; i++)
                {
                    for (int j = 0; j < common.height; j++)
                    {
                        cs[i*common.height + j] = TextureSource.Texture.GetPixel(common.x + i, common.y + j) + pp.Color;
                    }
                }
            }

            //Apply color
            TextureSource.Texture.SetPixels(common.x, common.y, common.width, common.height, cs);

            //Set up this chunk as ready to be updated on next Update()
            painted = true;
            
        }

        public virtual void Update()
        {
            if(painted)
            {
                TextureSource.Texture.Apply();
                painted = false;
            }
            
        }
    }
}

