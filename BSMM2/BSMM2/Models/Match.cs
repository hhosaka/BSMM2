using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public class Match {

		private class ByePlayer : Player {
			public override bool Dropped => true;

			public ByePlayer() : base("BYE") {
			}
		}

		[JsonObject]
		private class Record : IMatchRecord {

			[JsonProperty]
			public IPlayer Player { get; }

			[JsonProperty]
			public IResult Result { get; private set; }

			public void SetResult(IResult result)
				=> Result = result;

			public Record(IPlayer player) {
				Player = player;
			}
		}

		[JsonProperty]
		public readonly IPlayer BYE = new ByePlayer();

		[JsonProperty]
		private Record[] _records;

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !_records.Any(record => record.Result?.IsFinished != true);

		[JsonIgnore]
		public bool IsByeMatch
			=> _records.Any(result => result.Player == BYE);

		[JsonIgnore]
		public IEnumerable<string> PlayerNames
			=> _records.Select(result => result.Player.Name);

		[JsonIgnore]
		public IPlayer Player1 => _records[0].Player;

		[JsonIgnore]
		public IPlayer Player2 => _records[1].Player;

		[JsonIgnore]
		public RESULT_T? Player1Result => _records[0]?.Result?.RESULT;

		[JsonIgnore]
		public IEnumerable<IMatchRecord> Records => _records.Cast<IMatchRecord>();

		public void SetResults((IResult player1, IResult player2) points) {
			SetResults(points.player1, points.player2);
		}

		public void SetResults(IResult result1, IResult result2) {
			_records[0].SetResult(result1);
			_records[1].SetResult(result2);
		}

		public void Swap(Match other) {
			var temp = _records[0];
			_records[0] = other._records[0];
			other._records[0] = temp;
		}

		public void Commit()
			=> _records.ForEach(result => result.Player.Commit(this));

		private Record GetPlayerRecord(IPlayer player)
			=> _records.First(r => r.Player == player);

		private Record GetOpponentRecord(IPlayer player)
			=> _records.First(r => r.Player != player);

		public IResult GetResult(IPlayer player)
			=> GetPlayerRecord(player)?.Result;

		public IPlayer GetOpponentPlayer(IPlayer player)
			=> GetOpponentRecord(player)?.Player;

		public IResult GetOpponentResult(IPlayer player)
			=> GetOpponentRecord(player)?.Result;

		public Match() {// For Serializer
		}

		public Match(IPlayer player1, IPlayer player2) {
			_records = new[] { new Record(player1), new Record(player2) };
			IsGapMatch = (player1.Result?.Point != player2.Result?.Point);
		}

		public Match(IPlayer player, Rule rule) {
			_records = new[] { new Record(player), new Record(BYE) };
			SetResults(rule.CreatePoints(RESULT_T.Win));
		}
	}
}