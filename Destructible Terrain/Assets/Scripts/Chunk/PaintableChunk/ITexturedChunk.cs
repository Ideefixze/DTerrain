using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public interface ITexturedChunk :IChunk
    {
        int SortingLayerID { get; set; }
        ITextureSource TextureSource { get; }
    }
}

