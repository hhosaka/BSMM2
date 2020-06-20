using BSMM2.Models.Matches;
using System;

namespace BSMM2.Models {

	public class PointComparer : IComparer {

		public string Name
			=> "Match Point";

		public string Description
			=> "合計得点";

		public bool Active {
			get => true;
			set => throw new InvalidOperationException();
		}

		public int Compare(Player p1, Player p2)
			=> p1.Point.MatchPoint - p2.Point.MatchPoint;
	}

	public class LifePointComparer : IComparer {

		public string Name
			=> "Life Point";

		public string Description
			=> "合計ライフポイント";

		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> ((p1.Point as IBSPoint)?.LifePoint ?? -1) - ((p2.Point as IBSPoint)?.LifePoint ?? -1);
	}

	public class OpponentMatchPointComparer : IComparer {

		public string Name
			=> "Opponent Match Point";

		public string Description
			=> "対戦相手の合計得点";

		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.OpponentPoint.MatchPoint - p2.OpponentPoint.MatchPoint;
	}

	public class OpponentLifePointComparer : IComparer {

		public string Name
			=> "Opponent Life Point";

		public string Description
			=> "対戦相手の合計ライフポイント";

		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> ((IBSPoint)p1.OpponentPoint).LifePoint - ((IBSPoint)p2.OpponentPoint).LifePoint;
	}

	public class WinPointComparer : IComparer {

		public string Name
			=> "Win Point";

		public string Description
			=> "合計勝利ポイント";

		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.Point.WinPoint - p2.Point.WinPoint);
	}

	public class OpponentWinPointComparer : IComparer {

		public string Name
			=> "Opponent Win Point";

		public string Description
			=> "対戦相手の合計勝利ポイント";

		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.OpponentPoint.WinPoint - p2.OpponentPoint.WinPoint);
	}
}