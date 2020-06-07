using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Match : INotifyPropertyChanged {
		internal IScore Score { get; private set; }

		private class DefaultResult : IResult, IPoint {
			public RESULT_T RESULT => RESULT_T.Progress;

			[JsonIgnore]
			public int Point => 0;

			[JsonIgnore]
			public int LifePoint => -1;

			[JsonIgnore]
			public double WinPoint => 0;

			[JsonIgnore]
			public bool IsFinished => false;

			[JsonIgnore]
			public string Information
				=> throw new System.NotImplementedException();

			public void ExportData(TextWriter writer) {
			}

			public void ExportTitle(TextWriter writer) {
			}

			public IPoint GetPoint() => this;
		}

		private class ByePlayer : Player {
			public override bool Dropped => true;

			public ByePlayer() : base("BYE") {
			}
		}

		[JsonObject]
		public class Record : IMatchRecord {
			private static readonly IResult _defaultResult = new DefaultResult();

			[JsonProperty]
			private Match _match;

			[JsonProperty]
			private bool _side;

			[JsonProperty]
			public IPlayer Player { get; private set; }

			[JsonProperty]
			public IResult _result;

			[JsonProperty]
			public RESULT_T Result => _result.RESULT;

			[JsonIgnore]
			public IPoint Point => _result;

			[JsonIgnore]
			public bool IsFinished => _result.IsFinished;

			public void SetResult(IResult result)
				=> _result = result;

			private Record() {
			}

			public Record(Match match, bool side, IPlayer player) {
				_match = match;
				_side = side;
				Player = player;
				_result = _defaultResult;
			}
		}

		[JsonProperty]
		protected Rule _rule;

		[JsonProperty]
		public readonly IPlayer BYE = new ByePlayer();

		[JsonProperty]
		public Record[] _records;

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !_records.Any(record => !record.IsFinished);

		[JsonIgnore]
		public bool IsByeMatch
			=> _records.Any(result => result.Player == BYE);

		[JsonIgnore]
		public IEnumerable<string> PlayerNames
			=> _records.Select(result => result.Player.Name);

		[JsonIgnore]
		public IMatchRecord Record1 => _records[0];

		[JsonIgnore]
		public IMatchRecord Record2 => _records[1];

		public event PropertyChangedEventHandler PropertyChanged;

		public abstract void SetResult(RESULT_T result);

		public void SetResults((IResult player1, IResult player2) points) {
			SetResults(points.player1, points.player2);
		}

		protected void SetResults(IResult result1, IResult result2) {
			_records[0].SetResult(result1);
			_records[1].SetResult(result2);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Record1)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Record2)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFinished)));
		}

		public RESULT_T GetResult()
			=> _records[0].Result;

		public void Swap(Match other) {
			var temp = _records[0];
			_records[0] = other._records[0];
			other._records[0] = temp;
		}

		public void Commit()
			=> _records.ForEach(result => result.Player.Commit(this));

		public Record GetPlayerRecord(IPlayer player)
			=> _records.First(r => r.Player == player);

		private Record GetOpponentRecord(IPlayer player)
			=> _records.First(r => r.Player != player);

		public RESULT_T GetResult(IPlayer player)
			=> GetPlayerRecord(player).Result;

		public IPlayer GetOpponentPlayer(IPlayer player)
			=> GetOpponentRecord(player)?.Player;

		public RESULT_T GetOpponentResult(IPlayer player)
			=> GetOpponentRecord(player).Result;

		public Match() {// For Serializer
		}

		public Match(Rule rule, IPlayer player1, IPlayer player2 = null) {
			_rule = rule;

			if (player2 != null) {
				_records = new[] { new Record(this, true, player1), new Record(this, false, player2) };
				IsGapMatch = (player1.Result.Point != player2.Result.Point);
			} else {
				_records = new[] { new Record(this, true, player1), new Record(this, false, BYE) };
				SetResult(RESULT_T.Win);
			}
		}
	}
}