using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// SimulatedChunk uses ISimulationAlgorithm to change it's contents
    /// </summary>
    public class SimulatedChunk : CollidableChunk, IChildChunk
    {
        public IChunk Parent { get; set; }
        protected ISimulationAlgorithm SimulationAlgorithm { get; set; }


        public override void Init()
        {
            base.Init();

            SimulationAlgorithm = GetComponent<ISimulationAlgorithm>();
        }


        /// <summary>
        /// Simulates a chunk.
        /// </summary>
        public void Simulate()
        {
            SimulationAlgorithm?.Simulate(ref columns);
            colliderChanged = true;

        }
    }
}
