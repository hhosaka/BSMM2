using BSMM2.Models;
using BSMM2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundLogPage : ContentPage {

		public RoundLogPage(IGame game, IRound round) {
			InitializeComponent();

			BindingContext = new RoundLogViewModel(round, showMatch);

			async void showMatch(Match match) {
				await Navigation.PushModalAsync(new NavigationPage(game.CreateMatchPage(match)));
				RoundListView.SelectedItem = null;
			}
		}
	}
}