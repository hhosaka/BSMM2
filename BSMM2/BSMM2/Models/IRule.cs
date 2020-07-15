﻿using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms;

namespace BSMM2.Models {

	[JsonObject]
	public interface IRule {
		IEnumerable<IComparer> Comparers { get; }

		string Name { get; }

		string Description { get; }

		Match CreateMatch(IPlayer player1, IPlayer player2 = null);

		ContentPage CreateMatchPage(Match match);

		ContentPage CreateRulePage(Game game);

		IRule Clone();

		string GetDescription(Player player);

		IExportablePoint Point(IEnumerable<IPoint> results);

		Comparer<Player> GetComparer(bool force);
	}
}