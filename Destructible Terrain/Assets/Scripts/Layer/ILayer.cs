using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Interface for a single Layer created of chunks.
    /// </summary>
    /// <typeparam name="ChunkType"></typeparam>
    public interface ILayer<T> where T:IChunk
    {
        int ChunkCountX { get; set; }
        int ChunkCountY { get; set; }

        List<T> Chunks { get; }
        void SpawnChunks();
    }
}

