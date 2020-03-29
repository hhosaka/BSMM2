using BSMM2.Models;
using System;
using System.Collections.Generic;

namespace BSMM2.ViewModels {

	public class RoundLogViewModel : BaseViewModel {
		public IEnumerable<Match> Round { get; }

		private Action<Match> _showMatch;

		public Match SelectedItem {
			get => null;
			set {
				if (value != null) {
					_showMatch?.Invoke(value);
				}
			}
		}

		public RoundLogViewModel(IRound round, Action<Match> showMatch) {
			Round = round;
			_showMatch = showMatch;
		}
	}
}