﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BSMM2.Models {

	[JsonObject]
	public class Player : IPlayer {

		private class DefaultResult : IResult {
			public RESULT_T? RESULT => RESULT_T.Matching;

			public int Point => 0;

			public int LifePoint => 0;

			public double WinPoint => 0;

			public bool IsFinished => false;

			public void ExportTitle(TextWriter writer) {
				writer.Write("Point, WinPoint, LifePoint");
			}

			public void ExportData(TextWriter writer) {
				writer.Write("0, 0, 0");
			}
		}

		private static readonly DefaultResult _defaultResult = new DefaultResult();

		private class Total : IResult {
			public int Point { get; }

			public int LifePoint { get; }

			public double WinPoint { get; }

			public RESULT_T? RESULT => RESULT_T.Matching;

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

			public void ExportTitle(TextWriter writer) {
				writer.Write("Status, Point, WinPoint, LifePoint");
			}

			public void ExportData(TextWriter writer) {
				writer.Write(Point);
				writer.Write(", ");
				writer.Write(WinPoint);
				writer.Write(", ");
				writer.Write(LifePoint);
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
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this)?.RESULT != RESULT_T.Win);

		public bool IsAllLoses
			=> _matches.Count() > 0 && !_matches.Any(match => match.GetResult(this)?.RESULT != RESULT_T.Lose);

		[JsonIgnore]
		public IResult Result { get; private set; }

		[JsonIgnore]
		public IResult OpponentResult { get; private set; }

		public int Order { get; set; }

		public void Commit(Match match)
			=> _matches.Add(match);

		public RESULT_T? GetResult(Player player)
			=> _matches.FirstOrDefault(m => m.GetOpponentPlayer(this) == player)?.GetResult(this)?.RESULT;

		public void CalcResult(Rule rule)
			=> Result = new Total(_matches.Select(match => match.GetResult(this)));

		public void CalcOpponentResult(Rule rule)
			=> OpponentResult = new Total(_matches.Select(match => match.GetOpponentPlayer(this).Result));

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