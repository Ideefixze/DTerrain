using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public enum PaintingMode
    {
        REPLACE_COLOR,
        ADD_COLOR,
        NONE
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
        public List<int> AffectedChildChunks; //0 means main layer

        public PaintingParameters(Shape shape, Vector2Int position, Color color, PaintingMode paintingMode = PaintingMode.REPLACE_COLOR, DestructionMode destructionMode = DestructionMode.NONE, List<int> affectedChildChunks=null)
        {
            Shape = shape;
            Position = position;
            Color = color;
            PaintingMode = paintingMode;
            DestructionMode = destructionMode;
            if (affectedChildChunks == null) AffectedChildChunks = new List<int>() { 0 };
            else AffectedChildChunks = affectedChildChunks;
        }
    }
}

