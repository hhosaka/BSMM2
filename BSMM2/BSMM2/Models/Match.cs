using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace BSMM2.Models {

	[JsonObject]
	public abstract class Match : INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		[JsonObject]
		public class Record : IRecord {

			private class DefaultResult : IResult {
				public RESULT_T RESULT => RESULT_T.Progress;

				[JsonIgnore]
				public int MatchPoint => 0;

				[JsonIgnore]
				public int LifePoint => -1;

				[JsonIgnore]
				public double WinPoint => 0;

				[JsonIgnore]
				public bool IsFinished => false;
			}

			private static readonly IResult _defaultResult = new DefaultResult();

			[JsonProperty]
			public IPlayer Player { get; private set; }

			[JsonProperty]
			public IResult Result { get; private set; }

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
		protected IRule _rule;

		[JsonProperty]
		public Record[] _records;

		[JsonProperty]
		public bool IsGapMatch { get; }

		[JsonIgnore]
		public bool IsFinished
			=> !_records.Any(record => !record.Result.IsFinished);

		[JsonIgnore]
		public bool IsByeMatch
			=> _records.Any(result => result.Player == _rule.BYE);

		[JsonIgnore]
		public IEnumerable<string> PlayerNames
			=> _records.Select(result => result.Player.Name);

		[JsonIgnore]
		public IRecord Record1 => _records[0];

		[JsonIgnore]
		public IRecord Record2 => _records[1];

		public abstract void SetResult(RESULT_T result);

		protected void SetResults(IResult result1, IResult result2) {
			_records[0].SetResult(result1);
			_records[1].SetResult(result2);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Record1)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Record2)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFinished)));
		}

		public void Swap(Match other) {
			var temp = _records[0];
			_records[0] = other._records[0];
			other._records[0] = temp;
		}

		public void Commit()
			=> _records.ForEach(result => result.Player.Commit(this));

		public Record GetRecord(IPlayer player)
			=> _records.First(r => r.Player == player);

		public Record GetOpponentRecord(IPlayer player)
			=> _records.First(r => r.Player != player);

		public Match() {// For Serializer
		}

		public Match(IRule rule, IPlayer player1, IPlayer player2 = null) {
			_rule = rule;

			if (player2 != null) {
				_records = new[] { new Record(player1), new Record(player2) };
				IsGapMatch = (player1.Point.MatchPoint != player2.Point.MatchPoint);
			} else {
				_records = new[] { new Record(player1), new Record(_rule.BYE) };
				SetResult(RESULT_T.Win);
			}
		}
	}
}