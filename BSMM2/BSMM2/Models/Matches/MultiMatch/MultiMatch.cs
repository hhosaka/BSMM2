using Newtonsoft.Json;
using System.Collections.Generic;

namespace BSMM2.Models.Matches.MultiMatch {

	[JsonObject]
	internal class MultiMatch : Match {

		[JsonProperty]
		private MultiMatchRule _rule;

		[JsonProperty]
		private IEnumerable<RESULT_T> _results;

		public MultiMatch() {
		}

		public MultiMatch(MultiMatchRule rule, IPlayer player1, IPlayer player2)
			: base(rule, player1, player2) {
			_rule = rule;
		}

		public override void SetResult(RESULT_T result) {
			//_results = results;//TODO
			SetResults(_rule.CreatePoints(result));
		}
	}
}