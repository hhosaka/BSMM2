using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject(nameof(Player))]
	public class Player {

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public bool Dropped { get; private set; }

		[JsonProperty]
		private IList<Match> _matches;

		[JsonIgnore]
		private IResult _result;

		[JsonIgnore]
		private IResult _opponentResult;

		[JsonIgnore]
		public IResult Result => _result;

		[JsonIgnore]
		public IResult OpponentResult => _opponentResult;

		[JsonIgnore]
		public bool HasByeMatch
			=> _matches.Any(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		public void Commit(Match match)
			=> _matches.Add(match);

		public void Drop()
			=> Dropped = true;

		public RESULT? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this)?.RESULT;

		public void CalcResult(Rule rule)
			=> _result = rule.Sum(_matches.Select(match => match.GetResult(this)));

		public void CalcOpponentResult(Rule rule)
			=> _opponentResult = rule.Sum(_matches.Select(match => match.GetOpponentResult(this)));

		public Player() {// For Serializer
		}

		public Player(string name) {
			_matches = new List<Match>();
			Name = name;
		}
	}
}