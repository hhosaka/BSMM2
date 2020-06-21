using BSMM2.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	internal class TheComparer : Comparer<Player> {
		private IEnumerable<IComparer> _compareres;
		private bool _force;

		public TheComparer(IEnumerable<IComparer> compareres, bool force) {
			_compareres = compareres;
			_force = force;
		}

		public override int Compare(Player p1, Player p2) {
			foreach (var c in _compareres.Where(c => _force || c.Active)) {
				var ret = c.Compare(p1, p2);
				if (ret != 0) return ret;
			}
			return 0;
		}
	}

	public class PreComparer : IComparer {

		[JsonIgnore]
		public string Label => throw new InvalidOperationException();

		[JsonIgnore]
		public bool Mandatory => true;

		[JsonIgnore]
		public bool Active {
			get => true;
			set => throw new InvalidOperationException();
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

	public class PostComparer : IComparer {

		[JsonIgnore]
		public string Label => throw new InvalidOperationException();

		[JsonIgnore]
		public bool Mandatory => true;

		[JsonIgnore]
		public bool Active {
			get => true;
			set => throw new InvalidOperationException();
		}

		public int Compare(Player p1, Player p2) {
			var result = p1.GetResult(p2);
			return result == RESULT_T.Win ? 1 : result == RESULT_T.Lose ? -1 : 0;
		}
	}

	public class PointComparer : IComparer {

		[JsonIgnore]
		public string Label => throw new InvalidOperationException();

		[JsonIgnore]
		public bool Mandatory => true;

		public bool Active {
			get => true;
			set { }
		}

		public int Compare(Player p1, Player p2)
			=> p1.Point.MatchPoint - p2.Point.MatchPoint;
	}

	[JsonObject]
	public class LifePointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseLifePoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.Point.LifePoint - p2.Point.LifePoint;
	}

	[JsonObject]
	public class OpponentMatchPointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseOpponentMatchPoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.OpponentPoint.MatchPoint - p2.OpponentPoint.MatchPoint;
	}

	[JsonObject]
	public class OpponentLifePointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseOpponentLifePoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> p1.OpponentPoint.LifePoint - p2.OpponentPoint.LifePoint;
	}

	[JsonObject]
	public class WinPointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseWinPoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.Point.WinPoint - p2.Point.WinPoint);
	}

	[JsonObject]
	public class OpponentWinPointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseOpponentWinPoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2)
			=> (int)Math.Floor(p1.OpponentPoint.WinPoint - p2.OpponentPoint.WinPoint);
	}
}