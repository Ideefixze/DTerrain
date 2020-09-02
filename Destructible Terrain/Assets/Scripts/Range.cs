using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace DTerrain
{
    ///<summary>
    ///Range represents a single range: [min;max]
    ///</summary>
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

        public Range(Range r)
        {
            min = r.min;
            max = r.max;
        }


        /// <returns>Absolute length of a range.</returns>
        public int Length()
        {
            int len = Mathf.Abs(max - min);
            if (len <= 0) return 0;
            else return len;
        }

        /// <summary>
        /// For debugging.
        /// </summary>
        public void PrintRange()
        {
            Debug.Log("Range: [ " + min + " , " + max + " ]");
        }

        public static Range operator+(Range r, int a)
        {
            return new Range(r.min+a,r.max+a);
        }

        public static Range operator -(Range r, int a)
        {
            return new Range(r.min - a, r.max - a);
        }

    }
}