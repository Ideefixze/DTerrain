using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Simple destructor that creates a circle shaped destruction at given point with an outline.
    /// </summary>
    public class WormsDestructor : IDestructor
    {
        private Shape destroyShape;
        private List<Column> outlineColumns; //We should remember the outline as a "ring" not as primitive Shape (it uses only ranges, not columns)
        private int offset = 0;
        private float multiplier=0f;

        /// <summary>
        /// Constructor for WormsDestructor. Simple destructor that creates a circle shaped destruction at given point with an outline.
        /// </summary>
        /// <param name="destructionRadius">Radius of a destruction circle.</param>
        /// <param name="outlineRadius">Radius of an outline circle</param>
        /// <param name="multiplier">How darker than original texture outline is [0;1]</param>
        public WormsDestructor(int destructionRadius, int outlineRadius, float multiplier=0f)
        {
            this.multiplier = multiplier;

            //Create shapes
            destroyShape = Shape.GenerateShapeCircle(destructionRadius);
            Shape outlineShape = Shape.GenerateShapeCircle(outlineRadius);

            //And outline shape as columns...
            outlineColumns=new List<Column>();
            int k = 0;
            foreach (var range in outlineShape.ranges)
            {
                outlineColumns.Add(new Column(k));
                outlineColumns[k].AddRange(new Range(range.min, range.max));
                k++;
            }


            //"Cut" a circle in a bigger circle to create ring for an outline
            offset = outlineRadius - destructionRadius;
            k = 0;
            for (int i = offset; k < destroyShape.ranges.Count; i++)
            {
                outlineColumns[i].DelRange(destroyShape.ranges[k] + offset);
                k++;
            }
        }

        public void Destroy(int x, int y, World w)
        {
            w.DestroyTerrain(x, y, destroyShape);
            int k = 0;
            foreach (var c in outlineColumns)
            {
                foreach (var r in c.ranges)
                {
                    for (int i = r.min; i < r.max; i++)
                    {
                        Color col = w.ColorAt(x + k - offset, y + i - offset);
                        col.r = col.r * multiplier;
                        col.g = col.g * multiplier;
                        col.b = col.b * multiplier;
                        w.MakeOutline(x+k-offset,y+i-offset, col);
                    }
                }

                k++;

            }
        }

    }
}

