using System;
using System.Collections.Generic;
using System.Text;

namespace BSMM2.Models
{
    public interface IPoint
    {
        IPoint CreateOpponentPoint();

        int Compare(IPoint point, int level);
    }
}