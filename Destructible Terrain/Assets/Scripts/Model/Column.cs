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
        /// <returns>Index of the first range that contains a point or -1 if none has it</returns>
        public int IndexWithin(int point)
        {
            for (int i = 0; i < Ranges.Count; i++)
            {
                if (Ranges[i].isWithin(point) == true) return i;
            }
            return -1;
        }

        /// <summary>
        /// True if every row in [y0;y1] belongs to some range (ranges that touch each other count together).
        /// </summary>
        public bool Covers(int y0, int y1)
        {
            int cur = y0;
            while (cur <= y1)
            {
                //Furthest reach of any range that contains cur
                int next = cur;
                for (int k = 0; k < Ranges.Count; k++)
                {
                    Range r = Ranges[k];
                    if (r.Min <= cur && r.Max >= cur && r.Max + 1 > next) next = r.Max + 1;
                }
                if (next == cur) return false; //cur is air
                cur = next;
            }
            return true;
        }

        /// <summary>
        /// True if at least one row in [y0;y1] belongs to some range.
        /// </summary>
        public bool Touches(int y0, int y1)
        {
            for (int k = 0; k < Ranges.Count; k++)
            {
                Range r = Ranges[k];
                if (Mathf.Max(r.Min, y0) <= Mathf.Min(r.Max, y1)) return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes a single pixel/position from the column. Splits range into two if needed.
        /// </summary>
        /// <param name="pos">Position in column</param>
        public void SingleDelRange(int pos)
        {
            int found = IndexWithin(pos);
            if (found >= 0)
            {
                Range r = Ranges[found];
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
                Range sum;
                if (Range.TrySum(addr, Ranges[i], out sum))
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