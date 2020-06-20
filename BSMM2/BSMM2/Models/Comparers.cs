using Newtonsoft.Json;
using System;

namespace BSMM2.Models {

	[JsonObject]
	public class PointComparer : IComparer {

		[JsonProperty]
		public bool Active {
			get => true;
			set { }
		}

		public int Compare(Player p1, Player p2)
			=> p1.Point.MatchPoint - p2.Point.MatchPoint;
	}

	[JsonObject]
	public class LifePointComparer : IComparer {

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.Point.LifePoint - p2.Point.LifePoint;
	}

	[JsonObject]
	public class OpponentMatchPointComparer : IComparer {

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.OpponentPoint.MatchPoint - p2.OpponentPoint.MatchPoint;
	}

	[JsonObject]
	public class OpponentLifePointComparer : IComparer {

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.OpponentPoint.LifePoint - p2.OpponentPoint.LifePoint;
	}

	[JsonObject]
	public class WinPointComparer : IComparer {

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.Point.WinPoint - p2.Point.WinPoint);
	}

	[JsonObject]
	public class OpponentWinPointComparer : IComparer {

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.OpponentPoint.WinPoint - p2.OpponentPoint.WinPoint);
	}
}