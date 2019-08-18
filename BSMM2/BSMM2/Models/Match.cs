using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public class Match {

		[JsonObject]
		private class Record {

			[JsonProperty]
			public Player Player { get; }

			[JsonProperty]
			public IResult Point { get; private set; }

			public void SetPoint(IResult point)
				=> Point = point;

			public Record(Player player) {
				Player = player;
			}
		}

		[JsonProperty]
		public readonly Player BYE = new Player("BYE");

		[JsonProperty]
		private Record[] _records;

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !_records.Any(result => result.Point == null);

		[JsonIgnore]
		public bool IsByeMatch
			=> _records.Any(result => result.Player == BYE);

		[JsonIgnore]
		public IEnumerable<string> PlayerNames
			=> _records.Select(result => result.Player.Name);

		public void SetPoint((IResult, IResult) points) {
			_records[0].SetPoint(points.Item1);
			_records[1].SetPoint(points.Item2);
		}

		public void Swap(Match other) {
			var temp = _records[0];
			_records[0] = other._records[0];
			other._records[0] = temp;
		}

		public void Commit()
			=> _records.ForEach(result => result.Player.Commit(this));

		private Record GetPlayerRecord(Player player)
			=> _records.First(r => r.Player == player);

		private Record GetOpponentRecord(Player player)
			=> _records.First(r => r.Player != player);

		public IResult GetResult(Player player)
			=> GetPlayerRecord(player)?.Point;

		public Player GetOpponentPlayer(Player player)
			=> GetOpponentRecord(player)?.Player;

		public IResult GetOpponentResult(Player player)
			=> GetOpponentRecord(player)?.Point;

		public Match() {// For Serializer
		}

		public Match(Player player1, Player player2) {
			_records = new[] { new Record(player1), new Record(player2) };
			IsGapMatch = (player1.Result?.Point != player2.Result?.Point);
		}

		public Match(Player player) {
			_records = new[] { new Record(player), new Record(BYE) };
		}
	}
}