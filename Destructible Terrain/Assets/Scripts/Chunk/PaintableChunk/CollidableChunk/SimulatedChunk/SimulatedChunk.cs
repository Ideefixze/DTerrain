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
    public class SimulatedChunk : CollidableChunk, IChildChunk
    {
        public IChunk Parent { get; set; }

        public void Simulate()
        {

        }

        
    }
}
