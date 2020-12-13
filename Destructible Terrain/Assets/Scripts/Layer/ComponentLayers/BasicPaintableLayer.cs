using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class BasicPaintableLayer : PaintableLayer<PaintableChunk>
    {
        //CHUNK SIZE X!!!!
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
