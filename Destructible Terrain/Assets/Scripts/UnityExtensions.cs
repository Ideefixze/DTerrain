
using UnityEngine;
namespace DTerrain
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Gets a Rect that is an intersection of two Rects.
        /// </summary>
        /// <param name="r1">Rect 1</param>
        /// <param name="r2">Rect 2</param>
        /// <param name="area">Common area</param>
        /// <returns>True if r1 and r2 intersect</returns>
        public static bool Intersects(this RectInt r1, RectInt r2, out RectInt area)
        {
            area = new RectInt();

            if (r2.Overlaps(r1))
            {
                int x1 = Mathf.Min(r1.xMax, r2.xMax);
                int x2 = Mathf.Max(r1.xMin, r2.xMin);
                int y1 = Mathf.Min(r1.yMax, r2.yMax);
                int y2 = Mathf.Max(r1.yMin, r2.yMin);
                area.x = Mathf.Min(x1, x2);
                area.y = Mathf.Min(y1, y2);
                area.width = Mathf.Max(0, x1 - x2);
                area.height = Mathf.Max(0, y1 - y2);

                return true;
            }

            return false;
        }

    }

}
