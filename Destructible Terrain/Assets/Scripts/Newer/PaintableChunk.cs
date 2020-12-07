using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class PaintableChunk : MonoBehaviour, ITexturedChunk
    {
        private SpriteRenderer spriteRenderer;
        public ITextureSource TextureSource { get; set; }
        protected bool painted=false;


        public virtual void Init()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(TextureSource.Texture, new Rect(0, 0, TextureSource.Texture.width, TextureSource.Texture.height), new Vector2(0.5f, 0.5f), TextureSource.PPU);
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

