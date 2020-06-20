using Newtonsoft.Json;
using System;

namespace BSMM2.Models {

	public class BasicComparer : IComparer {

		[JsonProperty]
		public bool Active {
			get => true;
			set { }
		}

		public int Compare(Player p1, Player p2) {
			if (p1 == p2) {
				return 0;
			} else if (p1.Dropped) {
				return p2.Dropped ? 0 : -1;
			} else {
				return p2.Dropped ? 1 : 0;
			}
		}
	}

	public class PointComparer : IComparer {

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