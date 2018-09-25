using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2.Models
{
    internal class NullPoint : IPoint
    {
        public static readonly IPoint Instance = new NullPoint();

        public int MaxLevel => 0;

        public int Compare(IPoint point, int level) => 0;

        public IPoint CreateOpponentPoint() => null;
    }
}