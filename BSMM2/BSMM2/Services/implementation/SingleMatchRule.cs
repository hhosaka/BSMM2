using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BSMM2.Models;
using Xamarin.Forms;

namespace BSMM2.Services.implementation
{
    public class SingleMatchRule : Rule
    {
        private static Func<IPoint, IPoint, int>[] _compareres = new Func<IPoint, IPoint, int>[]
            {
                (x,y)=>((SingleMatchPoint)x).Point - ((SingleMatchPoint)y).Point
            };

        public int MaxLevel => SingleMatchPoint.MaxLevel;
        public ContentPage ContentPage { get; } = new ContentPage();//TODO TBD
    }
}