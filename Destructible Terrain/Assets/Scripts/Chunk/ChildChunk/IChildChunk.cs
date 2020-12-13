using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTerrain
{
    public interface IChildChunk : IChunk
    {
        IChunk Parent { get; set; }
    }
}
