using BSMM2.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	internal class CompUtil {

		public static int Comp2Factor(bool param1, bool param2) => param1 ? param2 ? 0 : -1 : param2 ? 1 : 0;
	}

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
			switch (p1.GetResult(p2)) {
				case RESULT_T.Win:
					return 1;

				case RESULT_T.Lose:
					return -1;

				default:
					var ret = CompUtil.Comp2Factor(p1.HasByeMatch, p2.HasByeMatch);
					if (ret == 0) {
						ret = CompUtil.Comp2Factor(p1.HasGapMatch, p2.HasGapMatch);
					}
					return ret;
			}
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

		public int Compare(Player p1, Player p2) {
			var value = p1.Point.WinPoint - p2.Point.WinPoint;
			return (int)(value > 0.0 ? Math.Ceiling(value) : Math.Floor(value));
		}
	}

	[JsonObject]
	public class OpponentWinPointComparer : IComparer {

		[JsonIgnore]
		public string Label => AppResources.LabelUseOpponentWinPoint;

		[JsonIgnore]
		public bool Mandatory => false;

		[JsonProperty]
		public bool Active { get; set; } = true;

		public int Compare(Player p1, Player p2) {
			var value = p1.OpponentPoint.WinPoint - p2.OpponentPoint.WinPoint;
			return (int)(value > 0.0 ? Math.Ceiling(value) : Math.Floor(value));
		}
	}
}