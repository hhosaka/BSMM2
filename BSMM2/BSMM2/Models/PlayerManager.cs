using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSMM2.Models
{
    class PlayerManager
    {
        private readonly IList<Player> _players = new List<Player>();
        
        public void Initialize(int num)
        {
            //TODO TBD
        }

        public void Add(string name)
        {
            //TODO TBD
        }

        public void Remove(int index)
        {
            //TODO TBD
        }

        class Compare : IComparer<Player>
        {
            int IComparer<Player>.Compare(Player x, Player y)
            {
                return 0;
            }
        }

        private static readonly IComparer<Player> _compare = new Compare();
        private static readonly IComparer<Player> _compare_id = new Compare();

        public IEnumerable<Player> GetPlayers(IComparer<Player> comparer)
        {
            return _players.OrderBy(p => p, comparer);
        }
    }
}
