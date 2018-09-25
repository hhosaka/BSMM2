using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models
{
    public class Player
    {
        public bool Dropped { get; set; }
        public string Name { get; set; }
        public IList<Match> Matches { get; } = new List<Match>();

        public IPoint Point
        {
            get
            {
                if (Matches.Any())
                {
                    return Matches.FirstOrDefault().GetPoint(this);//TODO tentative
                }
                return NullPoint.Instance;
            }
        }

        private bool HasByeMatch => false;//TODO TBD
        private bool HasGapMatch => false;//TODO TBD

        public bool HasMatched(Player player)
        {
            return Matches.Any(match => match.HasMatch(player));
        }

        public Player(string name)
        {
            Name = name;
        }
    }
}