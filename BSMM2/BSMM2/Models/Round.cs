using BSMM2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models
{
    public class Round
    {
        private readonly IEnumerable<Match> _matches;

        public static Round CreateInstance(Rule rule, IEnumerable<Player> players, int count = 10)
        {
            for (int i = 0; i < count; ++i)
            {
                var matches = MakeMatch(rule.GetPlayer(players));
                if (matches != null)
                {
                    return new Round(matches);
                }
            }
            return null;
        }

        private static IEnumerable<Match> MakeMatch(IEnumerable<Player> players)
        {
            var results = new Stack<Match>();
            var stack = new List<Player>();

            foreach (var p1 in players)
            {
                if (!p1.Dropped)
                {
                    foreach (var p2 in stack)
                    {
                        if (Matching(p1, p2))
                        {
                            stack.Remove(p2);
                            results.Push(new Match(p1, p2));
                            break;
                        }
                    }
                    stack.Add(p1);
                }
            }
            switch (stack.Count)
            {
                case 0:
                    return results;

                case 1:
                    results.Push(new Match(stack.First()));
                    return results;

                default:
                    return null;
            }

            bool Matching(Player p1, Player p2)
            {
                return !p1.HasMatched(p2);//TODO
            }
        }

        private Round(IEnumerable<Match> matches)
        {
            _matches = matches;
        }
    }
}