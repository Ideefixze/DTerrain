using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTerrain
{
    public interface IChunkCollider
    {
        void UpdateColliders(List<Column> pixelData, ITextureSource textureSource);
    }
}
