using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using System.Diagnostics;
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

			BindingContext = viewModel = new PlayersViewModel(app, NewGame, OpenRule, SelectGame, DeleteGame, AddPlayer);

			void NewGame()
				=> Navigation.PushModalAsync(new NavigationPage(new NewGamePage(_app)));
			void OpenRule()
				=> Navigation.PushModalAsync(new NavigationPage(new RulePage(_app)));
			void SelectGame()
				=> Navigation.PushModalAsync(
					new NavigationPage(new GamesPage(_app, "Select Item", selectGame)));
			void DeleteGame()
				=> Navigation.PushModalAsync(
					new NavigationPage(new GamesPage(_app, "Delete Item", deleteGame)));
			void AddPlayer()
				=> Navigation.PushModalAsync(new NavigationPage(new AddPlayerPage(_app)));

			async void selectGame(IGame game) {
				Debug.Assert(game != null);
				if (app.Select(game)) {
					MessagingCenter.Send<object>(this, Messages.REFRESH);
					await Navigation.PopModalAsync();
				}
			}

			async void deleteGame(IGame game) {
				Debug.Assert(game != null);
				if (app.Remove(game)) {
					MessagingCenter.Send<object>(this, Messages.REFRESH);
					await Navigation.PopModalAsync();
				}
			}
		}

		private void Log(object sender, EventArgs e) {
			DisplayAlert("log", new SerializeUtil().Log(), "Finish");
		}

		private void OpenSettingsPage(object sender, EventArgs e)
				=> Navigation.PushModalAsync(new NavigationPage(new SettingsPage(_app)));

		private void OpenJsonPage(object sender, EventArgs e)
				=> Navigation.PushModalAsync(new NavigationPage(new JsonPage(_app)));

		private async void OnPlayerTapped(object sender, ItemTappedEventArgs args) {
			if (args.Item is Player player)
				await Navigation.PushModalAsync(new NavigationPage(new PlayerPage(_app, player)));
			PlayersListView.SelectedItem = null;
		}
	}
}