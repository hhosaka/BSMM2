using BSMM2.Models;
using BSMM2.Models.Matches.SingleMatch;
using BSMM2.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace BSMM2Test {

	[TestClass]
	public class ViewModelTest {

		[TestMethod]
		public async Task ControlGameTest() {
			var app = new BSMMApp(new Engine());
			var viewModel = new PlayersViewModel(app);
			await viewModel.Refresh();
			app.Add(new FakeGame(new SingleMatchRule(), 8), true);
			await viewModel.Refresh();
			app.Remove(app.Game.Id);
			await viewModel.Refresh();
		}
	}
}