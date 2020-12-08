using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Sets up one layer as visible to mask.
    /// </summary>
    public class MaskedChunkAddon : MonoBehaviour, IChunkAddon
    {
        public void LoadAddon()
        {
            GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }
}
