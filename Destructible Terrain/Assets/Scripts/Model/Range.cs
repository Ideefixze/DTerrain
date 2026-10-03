using System;
using UnityEngine;

namespace DTerrain
{
    ///<summary>
    ///Range represents a single range: [min;max]
    ///
    ///A struct: two ints, no heap allocation per range. Equality is by value (Min and Max),
    ///which is what List.Remove / IndexOf / Contains in Column rely on.
    ///</summary>
    public struct Range : IEquatable<Range>
    {
        public int Min;
        public int Max;

        public int Length { 
            get
            {
                int len = Mathf.Abs(Max - Min);
                if (len <= 0) return 0;
                else return len;
            } 
        }

        public bool isWithin(int point)
        {
            return point <= Max && point >= Min;
        }

        public Range(int a, int b)
        {
            Min = a;
            Max = b;
        }

        public bool Equals(Range r)
        {
            return (Min == r.Min) && (Max == r.Max);
        }

        public override bool Equals(object obj)
        {
            if (obj is Range)
                return Equals((Range)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return Min * 397 ^ Max;
        }

        public static Range operator +(Range r, int a)
        {
            return new Range(r.Min + a, r.Max + a);
        }

        public static Range operator -(Range r, int a)
        {
            return new Range(r.Min - a, r.Max - a);
        }

        /// <summary>
        /// Joins two overlapping ranges. Ranges that do not overlap (adjacent ones included) are not joined.
        /// </summary>
        /// <returns>True and the joined range in sum; false (sum is default) if a and b do not overlap.</returns>
        public static bool TrySum(Range a, Range b, out Range sum)
        {
            if (a.Min <= b.Min && a.Max >= b.Max) // abBA = aA
            {
                sum = a;
                return true;
            }

            if (a.Min <= b.Min && a.Max >= b.Min) //abXAX
            {
                sum = new Range(a.Min, Mathf.Max(b.Max, a.Max));
                return true;
            }

            if (b.Min <= a.Min && b.Max >= a.Min) //baXAX
            {
                sum = new Range(b.Min, Mathf.Max(a.Max, b.Max));
                return true;
            }

            sum = new Range(0, 0);
            return false;
        }

    }
}
