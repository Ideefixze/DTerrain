using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTerrain
{
    /// <summary>
    /// Interface for all destructors
    /// </summary>
    public interface IDestructor
    {

        void Destroy(int x, int y, World w);

    }
}
