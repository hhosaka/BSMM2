using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
		public IExportablePoint Point { get; private set; }

		[JsonIgnore]
		public IExportablePoint OpponentPoint { get; private set; }

		[JsonIgnore]
		public int Order { get; set; }

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

		public void ExportTitle(TextWriter writer, string index = "") {
			writer.Write("Name, Dropped, ");
			Point.ExportTitle(writer);
			Point.ExportTitle(writer, "Op-");
			writer.Write("ByeCount, ");
			writer.WriteLine();
		}

		public void ExportData(TextWriter writer) {
			writer.Write("\"");
			writer.Write(Name);
			writer.Write("\", ");
			writer.Write(Dropped);
			writer.Write(", ");
			Point.ExportData(writer);
			OpponentPoint.ExportData(writer);
			writer.Write(ByeMatchCount);
			writer.Write(", ");
			writer.WriteLine();
		}

		public Player(IRule rule, string name) : this() {
			_rule = rule;
			_matches = new List<Match>();
			Name = name;
			Point = OpponentPoint = _rule.Point(Enumerable.Empty<IPoint>());
		}
	}
}