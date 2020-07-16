using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public class Player : IPlayer {

		[JsonProperty]
		private IRule _rule;

		[JsonProperty]
		public String Name { get; set; }

		[JsonProperty]
		public virtual bool Dropped { get; set; }

		[JsonProperty]
		private IList<Match> _matches;

		[JsonIgnore]
		public IEnumerable<Match> Matches => _matches;

		[JsonIgnore]
		public int ByeMatchCount
			=> _matches.Count(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		[JsonIgnore]
		public bool IsAllWins
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetRecord(this).Result.RESULT != RESULT_T.Win);

		[JsonIgnore]
		public bool IsAllLoses
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetRecord(this).Result.RESULT != RESULT_T.Lose);

		[JsonIgnore]
		public IPoint Point { get; private set; }

		[JsonIgnore]
		public IPoint OpponentPoint { get; private set; }

		[JsonIgnore]
		public int Order { get; set; }

		[JsonIgnore]
		public string Description
			=> _rule.GetDescription(this);

		public void Commit(Match match)
			=> _matches.Add(match);

		public int? GetResult(Player player) {
			var result = _matches.FirstOrDefault(m => m.GetOpponentRecord(this).Player == player)?.GetRecord(this).Result.RESULT;
			switch (result) {
				case null:
					return null;

				case RESULT_T.Win:
					return 1;

				case RESULT_T.Lose:
					return -1;

				default:
					return 0;
			}
		}

		internal void CalcPoint(IRule rule)
			=> Point = _rule.Point(_matches.Select(match => match.GetRecord(this).Result));

		internal void CalcOpponentPoint(IRule rule)
			=> OpponentPoint = _rule.Point(_matches.Select(match => (match.GetOpponentRecord(this).Player as Player)?.Point));

		public Player() {// For Serializer
		}

		public IDictionary<string, string> Export(IDictionary<string, string> data) {
			data["Name"] = string.Format("\"{0}\"", Name);
			data["Dropped"] = Dropped.ToString();
			Point.Export(data);
			OpponentPoint.Export(new Dictionary<string, string>()).ForEach(pair => data["Op-" + pair.Key] = pair.Value);
			data["ByeMatchCount"] = ByeMatchCount.ToString();
			return data;
		}

		public Player(IRule rule, string name) : this() {
			_rule = rule;
			_matches = new List<Match>();
			Name = name;
			Point = OpponentPoint = _rule.Point(Enumerable.Empty<IPoint>());
		}
	}
}