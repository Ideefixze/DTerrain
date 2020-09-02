using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Shape is a simple class that holds a list of Ranges (not ranges) and then is used to destroy terrain with.
    /// To make complicated shape destructions (not squares, circles etc.) don't use it as it supports only list of ranges.
    /// </summary>
    public class Shape
    {
        private int _width;
        private int _height;

        public List<Range> ranges;
        public Color outlineColor=Color.black;

        public int width { get => _width; private set => _width = value; }
        public int height { get => _height; private set => _height = value; }

        public Shape(int w, int h)
        {
            width = w;
            height = h;
            ranges = new List<Range>();
        }

        /// <summary>
        /// Generates a Shape: circle.
        /// </summary>
        /// <param name="r">Radius</param>
        /// <returns>Shape that looks like circle.</returns>
        public static Shape GenerateShapeCircle(int r)
        {
            int centerX = r;
            int centerY = r;
            Shape s = new Shape(2 * r, 2 * r);
            for (int i = 0; i <= 2 * r; i++)
            {
                bool down = false;
                int min = 0;
                int max = 0;
                for (int j = 0; j <= 2 * r; j++)
                {
                    if (Mathf.Sqrt((centerX - i) * (centerX - i) + (centerY - j) * (centerY - j)) < r)
                    {
                        if (down == false)
                        {
                            down = true;
                            min = j;
                        }

                    }
                    else
                    {
                        if (down)
                        {
                            max = j;
                            break;

                        }

                    }

                }
                if (down)
                {
                    Range range = new Range(min, max);
                    s.ranges.Add(range);
                }

            }

            return s;
        }
    }
}
