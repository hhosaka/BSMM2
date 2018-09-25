using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;

namespace BSMM2.Models
{
    public class Match
    {
        private readonly Player BYE = new Player("BYE");

        private class Result
        {
            public Player Player { get; }
            public IPoint Point { get; private set; } = NullPoint.Instance;

            public void SetPoint(IPoint point) => Point = point;

            public Result(Player player)
            {
                Player = player;
            }
        }

        private Result[] _results;

        public bool HasMatch(Player player)
        {
            return _results.Any(result => result.Player == player);
        }

        public void SetPoint(IPoint point)
        {
            _results[0].SetPoint(point);
            _results[1].SetPoint(point.CreateOpponentPoint());
        }

        public void Commit()
        {
            _results.ForEach(result => result.Player.Matches.Add(this));
        }

        private Player GetOpponent(Player player) => _results.First(r => r.Player != player).Player;

        public IPoint GetPoint(Player player)
        {
            return _results.First(r => r.Player == player)?.Point;
        }

        public Match(Player player1, Player player2)
        {
            _results = new[] { new Result(player1), new Result(player2) };
        }

        public Match(Player player)
        {
            _results = new[] { new Result(player), new Result(BYE) };
        }
    }
}