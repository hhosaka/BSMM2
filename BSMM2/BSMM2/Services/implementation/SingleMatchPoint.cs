using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2.Services.implementation
{
    public class SingleMatchPoint : IPoint
    {
        public int Point { get; }

        public IPoint CreateOpponentPoint()
        {
            switch (Point)
            {
                case 3:
                    return new SingleMatchPoint(0);

                case 0:
                    return new SingleMatchPoint(3);

                case 1:
                default:
                    return new SingleMatchPoint(1);
            }
        }

        public static int MaxLevel => 1;

        public int Compare(IPoint point, int level)
        {
            return Point - ((SingleMatchPoint)point).Point;
        }

        public SingleMatchPoint(int point)
        {
            Point = point;
        }
    }
}