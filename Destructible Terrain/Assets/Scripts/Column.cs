using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace DTerrain
{
    //Range represents a range [min;max]
    public class Range
    {
        public int min;
        public int max;

        public bool isWithin(int point)
        {
            return point <= max && point >= min;
        }

        public Range(int a, int b)
        {
            min = a;
            max = b;
        }

        public int Length()
        {
            int len = max - min;
            if (len <= 0) return 0;
            else return len;
        }

        public void PrintRange()
        {
            Debug.Log("Range: [ " + min + " , " + max + " ]");
        }

        public static Range operator+(Range r, int a)
        {
            return new Range(r.min+a,r.max+a);
        }

        
    }

    //Column represents a list of ranges: [ [min1;max1], [min2;max2] ... ].
    public class Column
    {
        public int x;
        public List<Range> ranges;
        public Column(int x) { this.x = x; ranges = new List<Range>(); }
        public Range AddRange(int mini, int maxi)
        {
            Range r = new Range(mini, maxi);
            ranges.Add(r);
            return r;
        }

        public Range AddRange(Range r)
        {
            ranges.Add(r);
            return r;
        }

        public bool isWithin(int point)
        {
            foreach (Range r in ranges)
            {
                if (r.isWithin(point) == true) return true;
            }
            return false;
        }

        public Range Within(int point)
        {
            foreach (Range r in ranges)
            {
                if (r.isWithin(point) == true) return r;
            }
            return null;
        }

        public void PrintColumn()
        {
            foreach (Range r in ranges)
            {
                r.PrintRange();
            }
        }

        //Deletes a single pixel/position from the column.
        //Splits the range into two if needed.
        public void SingleDelRange(int pos)
        {
            Range r = Within(pos);
            if (r != null)
            {
                Range r1 = new Range(r.min, pos - 1);
                Range r2 = new Range(pos + 1, r.max);
                if (r1.Length() > 0) AddRange(r1);
                if (r2.Length() > 0) AddRange(r2);
                ranges.Remove(r);
            }
        }

        //Deletes a range from column.
        //May be buggy for len=1 ranges.
        public bool DelRange(Range delr)
        {
            int a = delr.min;
            int b = delr.max;
            bool changed = false;
            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].min < a && ranges[i].max > b) ///0---a-----b----1
                {
                    changed = true;
                    if (Mathf.Abs(a - b) == 0)
                    {
                        ranges.Add(new Range(ranges[i].min, a - 1));
                        ranges.Add(new Range(b + 1, ranges[i].max));
                        ranges.Remove(ranges[i]);
                        break;
                    }
                    ranges.Add(new Range(ranges[i].min, a));
                    ranges.Add(new Range(b, ranges[i].max));
                    ranges.Remove(ranges[i]);
                    break;
                    
                }

                if (ranges[i].min >= a && ranges[i].max <= b) ///-------a-0---1--b
                {
                    changed = true;
                    ranges.Remove(ranges[i]);
                    i--;
                    continue;
                }

                if (ranges[i].min < a && ranges[i].max <= b && ranges[i].max > a) ///-------0--a---1---b
                {
                    changed = true;
                    ranges.Add(new Range(ranges[i].min, a));
                    ranges.Remove(ranges[i]);
                    i--;
                    continue;
                }

                if (ranges[i].min >= a && ranges[i].max > b && ranges[i].min < b) ///--a-0----b---1--
                {
                    changed = true;
                    ranges.Add(new Range(b, ranges[i].max));
                    ranges.Remove(ranges[i]);
                    i--;
                    continue;
                }
            }
            return changed;
        }

    }
}