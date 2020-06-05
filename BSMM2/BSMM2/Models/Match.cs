using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Match : INotifyPropertyChanged {

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
			public IPlayer Player { get; private set; }

			[JsonProperty]
			public IResult Result { get; private set; }

			[JsonIgnore]
			public IPoint Point => Result.GetPoint();

			public void SetResult(IResult result)
				=> Result = result;

			private Record() {
			}

			public Record(IPlayer player) {
				Player = player;
				Result = _defaultResult;
			}
		}

		[JsonProperty]
		public readonly IPlayer BYE = new ByePlayer();

		[JsonProperty]
		public Record[] _records;

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !_records.Any(record => !record.Result.IsFinished);

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
			=> _records[0].Result.RESULT;

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

		public Match(Rule rule, IPlayer player1, IPlayer player2 = null) {
			if (player2 != null) {
				_records = new[] { new Record(player1), new Record(player2) };
				IsGapMatch = (player1.Result?.GetPoint().Point != player2.Result?.GetPoint().Point);
			} else {
				_records = new[] { new Record(player1), new Record(BYE) };
				SetResults(rule.CreatePoints(RESULT_T.Win));
			}
		}
	}
}