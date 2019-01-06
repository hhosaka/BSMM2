using BSMM2.Modules.Rules;
using BSMM2.Modules.Rules.Match;
using BSMM2.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public class Rule {

		[JsonIgnore]
		public virtual IEnumerable<Func<IResult, IResult, int>> Compareres { get; }

		private class TheComparer : Comparer<Player> {
			private IEnumerable<Func<IResult, IResult, int>> _compareres;

			public TheComparer(IEnumerable<Func<IResult, IResult, int>> compareres) {
				_compareres = compareres;
			}

			public override int Compare(Player x, Player y) {
				if (x != y) {
					foreach (var comparer in _compareres) {
						var result = comparer(x.Result, y.Result);
						if (result != 0) {
							return result;
						}
					}
					return x.GetResult(y) ?? 0;
				}
				return 0;
			}
		}

		public enum RESULT { Win, Lose, Draw }

		public virtual ContentPage ContentPage { get; }

		public Comparer<Player> CreateComparer(int omitt = 0) {
			var count = Compareres.Count() - omitt;
			if (count > 0) {
				return new TheComparer(Compareres);
			}
			return null;
		}
	}
}