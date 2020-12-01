using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Interface for a single Layer created of chunks.
    /// </summary>
    /// <typeparam name="ChunkType"></typeparam>
    public interface ILayer<ChunkType> where ChunkType:IChunk
    {
        void InitChunk();
        void Paint(PaintingParameters paintingParameters);
    }
}

