using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DTerrain
{
    public class SandSimulationAlgorithm : MonoBehaviour, ISimulationAlgorithm
    {
        /// <summary>
        /// Generates a painting action list in form of list of PaintingParameters to create a simulation step.
        /// </summary>
        /// <param name="columns">Columns of simulated chunk</param>
        /// <param name="texture">Texture of simulated chunk</param>
        /// <returns>List of PaintingParameters that have to be applied to chunk in order for it to be simulated</returns>
        public void Simulate(ref List<Column> columns)
        {
            foreach (Column c in columns)
            {
                foreach (Range r in c.Ranges)
                {
                    r.Min = r.Min - 1;
                    r.Max = r.Max - 1;
                }
            }
        }

    }
}
