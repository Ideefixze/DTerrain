using System.Collections.Generic;
using UnityEngine;

namespace DTerrain
{
    /// <summary>
    /// Column represents a list of ranges: [ [min1;max1], [min2;max2] ... ].
    /// </summary>
    public class Column
    {
        /// <summary>
        /// X pos of an column. Unused, but can be useful in future.
        /// </summary>
        public int X;
        public List<Range> Ranges;
        public Column(int x) { this.X = x; Ranges = new List<Range>(); }
        public Range AddRange(int mini, int maxi)
        {
            Range r = new Range(mini, maxi);
            Ranges.Add(r);
            return r;
        }

        public Range AddRange(Range r)
        {
            Ranges.Add(r);
            return r;
        }

        public bool isWithin(int point)
        {
            foreach (Range r in Ranges)
            {
                if (r.isWithin(point) == true) return true;
            }
            return false;
        }


        /// <param name="point">Point of intrest</param>
        /// <returns>Range that contains a point or null if none has it</returns>
        public Range Within(int point)
        {
            foreach (Range r in Ranges)
            {
                if (r.isWithin(point) == true) return r;
            }
            return null;
        }

        /// <summary>
        /// Deletes a single pixel/position from the column. Splits range into two if needed.
        /// </summary>
        /// <param name="pos">Position in column</param>
        public void SingleDelRange(int pos)
        {
            Range r = Within(pos);
            if (r != null)
            {
                Range r1 = new Range(r.Min, pos - 1);
                Range r2 = new Range(pos + 1, r.Max);
                if (r1.Length > 0) AddRange(r1);
                if (r2.Length > 0) AddRange(r2);
                Ranges.Remove(r);
            }
        }

        /// <summary>
        /// Deletes a range from column. May be buggy for len=1 ranges.
        /// </summary>
        /// <param name="delr">Range to delete a column with</param>
        /// <returns>True if any changes were made</returns>
        /// 

        //TODO: TESTS TESTS
        public bool DelRange(Range delr)
        {
            int a = delr.Min;
            int b = delr.Max;
            bool changed = false;
            for (int i = 0; i < Ranges.Count; i++)
            {
                if (Ranges[i].Min < a && Ranges[i].Max > b) ///0---a-----b----1
                {
                    changed = true;
                    if (Mathf.Abs(a - b) == 0)
                    {
                        Ranges.Add(new Range(Ranges[i].Min, a - 1));
                        Ranges.Add(new Range(b + 1, Ranges[i].Max));
                        Ranges.Remove(Ranges[i]);
                        break;
                    }
                    Ranges.Add(new Range(Ranges[i].Min, a));
                    Ranges.Add(new Range(b, Ranges[i].Max));
                    Ranges.Remove(Ranges[i]);
                    break;

                }

                if (Ranges[i].Min >= a && Ranges[i].Max <= b) ///-------a-0---1--b
                {
                    changed = true;
                    Ranges.Remove(Ranges[i]);
                    i--;
                    continue;
                }

                if (Ranges[i].Min < a && Ranges[i].Max <= b && Ranges[i].Max > a) ///-------0--a---1---b
                {
                    changed = true;
                    Ranges.Add(new Range(Ranges[i].Min, a));
                    Ranges.Remove(Ranges[i]);
                    i--;
                    continue;
                }

                if (Ranges[i].Min >= a && Ranges[i].Max > b && Ranges[i].Min < b) ///--a-0----b---1--
                {
                    changed = true;
                    Ranges.Add(new Range(b, Ranges[i].Max));
                    Ranges.Remove(Ranges[i]);
                    i--;
                    continue;
                }
            }
            return changed;
        }

        public bool SumRange(Range addr)
        {
            bool summed = false;
            bool changed = false;
            for(int i = 0; i<Ranges.Count;i++)
            {
                Range sum = Range.Sum(addr, Ranges[i]);
                
                if (sum!=null)
                {
                    if (sum.Equals(Ranges[i]) == false) changed = true;
                    Ranges[i] = sum;
                    summed = true;
                    break;
                }

            }
            if(summed==false)
            {
                AddRange(addr);
                changed = true;
            }

            return changed;
        }

    }
}