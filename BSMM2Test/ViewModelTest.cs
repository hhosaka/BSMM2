﻿using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2Test {

	[TestClass]
	public class ViewModelTest {

		[TestMethod]
		public async Task ControlGameTest() {
			var app = BSMMApp.Create();
			var viewModel = new PlayersViewModel(app);
			await viewModel.Refresh();
			app.Add(new FakeGame(new SingleMatchRule(), 8), true);
			await viewModel.Refresh();

			MessagingCenter.Send<object>(app, "UpdatedRound");

			var app2 = new Engine().CreateApp();
			Assert.AreEqual(true, app2.IsValidGame);

			app2.Remove(app2.Game.Id);
			await viewModel.Refresh();

			MessagingCenter.Send<object>(app2, "UpdatedRound");

			Assert.AreEqual(false, new Engine().CreateApp().IsValidGame);
		}
	}
}