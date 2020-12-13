using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    public class NoChunkCollider : MonoBehaviour, IChunkCollider
    {

        public void UpdateColliders(List<Column> pixelData, ITextureSource textureSource)
        {
            //Doesnt do anything.
        }

    }
}
