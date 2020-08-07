using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    public class Shape
    {
        int _width;
        int _height;

        public List<Range> columns;

        public int width { get => _width; private set => _width = value; }
        public int height { get => _height; private set => _height = value; }

        public Shape(int w, int h)
        {
            width = w;
            height = h;
            columns = new List<Range>();
        }

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
                    s.columns.Add(range);
                }

            }

            return s;
        }
    }
}
