using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class PaintableChunk : MonoBehaviour, ITexturedChunk
    {
        public SpriteRenderer SpriteRenderer { get; private set; }
        public ITextureSource TextureSource { get; set; }
        protected bool painted=false;


        public virtual void Init()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.sprite = Sprite.Create(TextureSource.Texture, new Rect(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), new Vector2(0.5f, 0.5f), TextureSource.PPU);

            //TODO:change it to postinit
            foreach(IChunkAddon addon in GetComponents<IChunkAddon>())
            {
                addon.LoadAddon();
            }
        }

        public virtual void Paint(RectInt r, Color c)
        {
            RectInt common;
            r.Intersects(new RectInt(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), out common);
            int len = common.width * common.height;
            Color[] cs = new Color[len];
            for (int i = 0; i < len; i++)
                cs[i] = c;

            TextureSource.Texture.SetPixels(common.x, common.y, common.width, common.height, cs);

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

