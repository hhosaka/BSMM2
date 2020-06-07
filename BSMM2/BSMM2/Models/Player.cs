using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Player : IPlayer {
		private static readonly IResult _defaultResult = new TheResult(new Total());

		private class TheResult : IResult {
			private IPoint _point;

			public int Point => _point.Point;

			public int LifePoint => _point.LifePoint;

			public double WinPoint => _point.WinPoint;

			public RESULT_T RESULT => RESULT_T.Progress;

			public bool IsFinished => true;

			public IPoint GetPoint() => _point;

			public string Information
				=> "Point = " + Point + " /Life = " + ToLifePoint(LifePoint) + " /Win = " + WinPoint;

			private string ToLifePoint(int lifePoint)
				=> lifePoint >= 0 ? lifePoint.ToString() : "-";

			public void ExportTitle(TextWriter writer) {
				writer.Write("Point, WinPoint, LifePoint");
			}

			public void ExportData(TextWriter writer) {
				writer.Write(Point);
				writer.Write(", ");
				writer.Write(WinPoint);
				writer.Write(", ");
				writer.Write(LifePoint);
			}

			public TheResult(IPoint point) {
				_point = point;
			}
		}

		private class Total : IPoint {
			public int Point { get; }

			public int LifePoint { get; }

			public double WinPoint { get; }

			public Total() {
			}

			public Total(IEnumerable<IPoint> source) {
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
		public String Name { get; set; }

		[JsonProperty]
		public virtual bool Dropped { get; set; }

		[JsonProperty]
		private IList<Match> _matches;

		[JsonIgnore]
		public IEnumerable<Match> Matches => _matches;

		public bool HasByeMatch
			=> _matches.Any(match => match.IsByeMatch);

		public bool HasGapMatch
			=> _matches.Any(match => match.IsGapMatch);

		public bool IsAllWins
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this) != RESULT_T.Win);

		public bool IsAllLoses
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this) != RESULT_T.Lose);

		[JsonIgnore]
		public IResult Result { get; private set; }

		[JsonIgnore]
		public IPoint Point => (Result as TheResult).GetPoint();

		[JsonIgnore]
		public IPoint OpponentPoint => (OpponentResult as TheResult).GetPoint();

		[JsonIgnore]
		public IResult OpponentResult { get; private set; }

		public int Order { get; set; }

		public void Commit(Match match)
			=> _matches.Add(match);

		public RESULT_T? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this);

		internal void CalcResult(Rule rule)
			=> Result = new TheResult(new Total(_matches.Select(match => match.GetPlayerRecord(this).Point)));

		internal void CalcOpponentResult(Rule rule)
			=> OpponentResult = new TheResult(new Total(_matches.Select(match => match.GetOpponentPlayer(this).Result)));

		public Player() {// For Serializer
			Result = _defaultResult;
		}

		public void ExportTitle(TextWriter writer) {
			writer.Write("Name, Dropped, ");
			Result.ExportTitle(writer);
		}

		public void ExportData(TextWriter writer) {
			writer.Write("\"");
			writer.Write(Name);
			writer.Write("\", ");
			writer.Write(Dropped);
			writer.Write(", ");
			Result.ExportData(writer);
		}

		public Player(string name) : this() {
			_matches = new List<Match>();
			Name = name;
		}
	}
}