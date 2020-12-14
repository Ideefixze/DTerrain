using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTerrain
{
    public interface ISimulationAlgorithm
    {
        /// <summary>
        /// Generates a painting action list in form of list of PaintingParameters to create a simulation step.
        /// </summary>
        /// <param name="columns">Columns of simulated chunk</param>
        /// <param name="texture">Texture of simulated chunk</param>
        /// <returns>List of PaintingParameters that have to be applied to chunk in order for it to be simulated</returns>
        void Simulate(ref List<Column> columns);

    }
}
