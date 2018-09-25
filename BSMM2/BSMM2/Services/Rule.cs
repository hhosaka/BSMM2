using BSMM2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BSMM2.Services
{
    public abstract class Rule
    {
        private class Comparer : Comparer<IPoint>
        {
            private readonly int _level;

            public override int Compare(IPoint x, IPoint y)
            {
                return x.Compare(y, _level);
            }

            public Comparer(int level)
            {
                _level = level;
            }
        }

        private readonly Comparer _comparer;

        public IEnumerable<Player> GetPlayer(IEnumerable<Player> players, int level = 0, bool shuffle = true)
        {
            if (shuffle)
            {
                players = players.OrderBy(i => Guid.NewGuid());
            }
            return players.OrderByDescending(p => p.Point, new Comparer(level));
        }

        private ContentPage ContentPage { get; }
    }
}