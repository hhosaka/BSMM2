using BSMM2.Models;
using BSMM2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayersPage : ContentPage {
		private PlayersViewModel viewModel;
		private BSMMApp _app;

		public PlayersPage(BSMMApp app) {
			_app = app;
			InitializeComponent();

			BindingContext = viewModel = new PlayersViewModel(app, NewGame, OpenRule, SelectGame, AddPlayer);

			void NewGame()
				=> Navigation.PushModalAsync(new NavigationPage(new NewGamePage(_app)));
			void OpenRule()
				=> Navigation.PushModalAsync(new NavigationPage(new RulePage(_app)));
			void SelectGame()
				=> Navigation.PushModalAsync(new NavigationPage(new GamesPage(_app)));
			void AddPlayer()
				=> Navigation.PushModalAsync(new NavigationPage(new AddPlayerPage(_app)));
		}

		private async void OnPlayerTapped(object sender, SelectedItemChangedEventArgs args) {
			if (args.SelectedItem is Player player)
				await Navigation.PushAsync(new PlayerPage(_app, player));
			PlayersListView.SelectedItem = null;
		}
	}
}