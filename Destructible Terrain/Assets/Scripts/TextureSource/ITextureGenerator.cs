using System;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public interface ITextureGenerator
    {
        Texture2D GenerateTexture();
    }
}
