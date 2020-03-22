using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BSMM2Test {

	[TestClass]
	public class ViewModelTest {

		[TestMethod]
		public async Task ControlGameTest() {
			var app = BSMMApp.Create();
			var viewModel = new PlayersViewModel(app);
			await viewModel.ExecuteRefresh();
			app.Add(new FakeGame(new SingleMatchRule(), 8), true);
			await viewModel.ExecuteRefresh();

			MessagingCenter.Send<object>(app, Messages.REFRESH);

			var app2 = new Engine().CreateApp();
			Assert.AreNotEqual(Guid.Empty, app2.Game.Id);

			app2.Remove((Game)app2.Game);
			await viewModel.ExecuteRefresh();

			MessagingCenter.Send<object>(app2, Messages.REFRESH);

			Assert.AreEqual(Guid.Empty, new Engine().CreateApp().Game.Id);
		}
	}
}