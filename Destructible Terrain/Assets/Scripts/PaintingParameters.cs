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

    public enum DestructionMode
    {
        NONE,
        DESTROY,
        BUILD
    }

    public struct PaintingParameters
    {
        public Shape Shape;
        public Vector2Int Position;
        public Color Color;
        public PaintingMode PaintingMode;
        public DestructionMode DestructionMode;

        public PaintingParameters(Shape shape, Vector2Int position, Color color, PaintingMode paintingMode = PaintingMode.REPLACE_COLOR, DestructionMode destructionMode = DestructionMode.NONE)
        {
            Shape = shape;
            Position = position;
            Color = color;
            PaintingMode = paintingMode;
            DestructionMode = destructionMode;
        }
    }
}

