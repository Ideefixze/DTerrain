using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTerrain
{
    public class BasicSimulatedLayer : SimulatedLayer
    {
        public virtual void Start()
        {
            SpawnChunks();
            InitChunks();

        }

        public virtual void Update()
        {
        }
    }
}
