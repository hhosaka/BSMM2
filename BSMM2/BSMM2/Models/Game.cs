using BSMM2.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BSMM2.Models
{
    public class Game
    {
        private string _title = "Player";
        private readonly Rule _rule;
        private readonly List<Player> _players = new List<Player>();
        private readonly List<Round> _rounds = new List<Round>();

        public Game(Rule rule, int count)
        {
            _rule = rule;
            for (int i = 0; i < count; ++i)
            {
                _players.Add(new Player(string.Format("{0}{1:000}", _title, i + 1)));
            }
        }

        public Game(Rule rule, TextReader reader)
        {
            _rule = rule;
            string buf;
            while ((buf = reader.ReadLine()) != string.Empty)
            {
                _players.Add(new Player(buf));
            }
        }

        public void Add(Player player)
        {
            var index = _players.IndexOf(_players.Find(p => p.Name.CompareTo(player.Name) > 0));
            if (index == -1)
            {
                _players.Add(player);
            }
            else
            {
                _players.Insert(index, player);
            }
        }

        public IEnumerable<Player> GetPlayersByName()
        {
            return _players.OrderByDescending(player => player.Point);
        }

        public Round GetRound(bool shuffle = false)
        {
            return Round.CreateInstance(_rule, _players);
        }
    }
}