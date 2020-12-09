using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public enum PaintingMode
    {
        REPLACE_COLOR,
        ADD_COLOR
    }
    public struct PaintingParameters
    {
        public Shape Shape;
        public Vector2Int Position;
        public Color Color;
        public PaintingMode PaintingMode;
    }
}

