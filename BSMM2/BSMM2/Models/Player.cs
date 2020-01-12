using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Player : IPlayer {

		private class Total : IResult {
			public int Point { get; }

			public int LifePoint { get; }

			public double WinPoint { get; }

			public RESULT_T? RESULT => null;

			public bool IsFinished => true;

			public Total(IEnumerable<IResult> source) {
				foreach (var point in source) {
					if (point != null) {
						Point += point.Point;
						LifePoint += point.LifePoint;
						WinPoint += point.WinPoint;
					}
				}
				WinPoint = source.Any() ? WinPoint / source.Count() : 0.0;
			}
		}

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public virtual bool Dropped { get; private set; }

		[JsonProperty]
		private IList<Match> _matches;

		public bool HasByeMatch
			=> _matches.Any(match => match.IsByeMatch);

		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		public bool IsAllWins
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this).RESULT != RESULT_T.Win);

		public bool IsAllLoses
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this).RESULT != RESULT_T.Lose);

		public IResult Result { get; private set; }

		public IResult OpponentResult { get; private set; }

		public int Order { get; set; }

		public void Commit(Match match)
			=> _matches.Add(match);

		public void Drop()
			=> Dropped = true;

		public RESULT_T? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this)?.RESULT;

		public void CalcResult(Rule rule)
			=> Result = new Total(_matches.Select(match => match.GetResult(this)));

		public void CalcOpponentResult(Rule rule)
			=> OpponentResult = new Total(_matches.Select(match => match.GetOpponentPlayer(this).Result));

		public Player() {// For Serializer
		}

		public Player(string name) {
			_matches = new List<Match>();
			Name = name;
		}
	}
}