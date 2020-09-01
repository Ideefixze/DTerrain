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
        public WormsDestructor(int destructionRadius, int outlineRadius)
        {
            destroyShape = Shape.GenerateShapeCircle(destructionRadius);
            Shape outlineShape = Shape.GenerateShapeCircle(outlineRadius);

            outlineColumns=new List<Column>();
            int k = 0;
            foreach (var range in outlineShape.ranges)
            {
                outlineColumns.Add(new Column(k));
                outlineColumns[k].AddRange(new Range(range.min, range.max));
                k++;
            }


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
                        w.MakeOutline(x+k-offset,y+i-offset, Color.black);
                    }
                }

                k++;

            }
        }

    }
}

