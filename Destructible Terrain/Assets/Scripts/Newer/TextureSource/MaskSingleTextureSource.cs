using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Basic SingleTextureSource that has only one texture and keeps a copy of starting state of that texture.
    /// </summary>
    [RequireComponent(typeof(SpriteMask), typeof(SpriteRenderer))]
    public class MaskSingleTextureSource: SingleTextureSource
    {
        public override void SetUpToRenderer(SpriteRenderer spriteRenderer)
        {
            GetComponent<SpriteMask>().sprite = spriteRenderer.sprite;
        }
    }
}
