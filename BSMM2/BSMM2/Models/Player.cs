using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Player : IPlayer {

		[JsonProperty]
		private Rule _rule;

		[JsonProperty]
		public String Name { get; set; }

		[JsonProperty]
		public virtual bool Dropped { get; set; }

		[JsonProperty]
		private IList<Match> _matches;

		[JsonIgnore]
		public IEnumerable<Match> Matches => _matches;

		[JsonIgnore]
		public bool HasByeMatch
			=> _matches.Any(match => match.IsByeMatch);

		[JsonIgnore]
		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		[JsonIgnore]
		public bool IsAllWins
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this) != RESULT_T.Win);

		[JsonIgnore]
		public bool IsAllLoses
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this) != RESULT_T.Lose);

		[JsonIgnore]
		public IPoint Point { get; private set; }

		[JsonIgnore]
		public IPoint OpponentPoint { get; private set; }

		[JsonIgnore]
		public int Order { get; set; }

		[JsonIgnore]
		public string Information
			=> Point.Information;

		public void Commit(Match match)
			=> _matches.Add(match);

		public RESULT_T? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this);

		internal void CalcResult(Rule rule)
			=> Point = _rule.Point(_matches.Select(match => match.GetPlayerRecord(this).Point));

		internal void CalcOpponentResult(Rule rule)
			=> OpponentPoint = _rule.Point(_matches.Select(match => match.GetOpponentPlayer(this).Point));

		public Player() {// For Serializer
		}

		public void ExportTitle(TextWriter writer) {
			writer.Write("Name, Dropped, ");
			Point.ExportTitle(writer);
		}

		public void ExportData(TextWriter writer) {
			writer.Write("\"");
			writer.Write(Name);
			writer.Write("\", ");
			writer.Write(Dropped);
			writer.Write(", ");
			Point.ExportData(writer);
		}

		public Player(Rule rule, string name) : this() {
			_rule = rule;
			Name = name;
			_matches = new List<Match>();
			Point = _rule.Point(Enumerable.Empty<IPoint>());
		}
	}
}