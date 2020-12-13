using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public interface IChunk
    {
        void Init();
        void Update();

        bool IsOccupied(Vector2Int at);
    }
}

