using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// CollidableChunk on each Paint execution deletes data from a list of columns, that is used to create BoxColliders2D.
    /// </summary>
    public class ComplexChunk : CollidableChunk
    {

        public List<IChildChunk> ChildChunks { get; private set; }

        public override void Init()
        {
            base.Init();
            ChildChunks = new List<IChildChunk>();
        }

        public override bool IsOccupiedAt(Vector2Int at)
        {
            return base.IsOccupiedAt(at) || ChildChunks.Any(c => c.IsOccupied(at));
        }

    }
}
