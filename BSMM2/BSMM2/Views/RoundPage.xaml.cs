using BSMM2.Models;
using BSMM2.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundPage : ContentPage {
		private RoundViewModel viewModel;

		public RoundPage(BSMMApp app) {
			InitializeComponent();

			BindingContext = viewModel = new RoundViewModel(app, showRoundsLog);
			viewModel.OnFailedMatching += OnMatchingFailed;

			void showRoundsLog()
				=> Navigation.PushModalAsync(new RoundsLogPage(app));
		}

		private async Task OnMatchingFailed() {
			await DisplayAlert("Alert", "Fail to generate match", "OK");//TODO : guide to match settings
		}

		private async void OnMatchTapped(object sender, ItemTappedEventArgs args) {
			if (args.Item is Match match && viewModel.Game.ActiveRound.IsPlaying)
				await Navigation.PushModalAsync(new NavigationPage(viewModel.Game.CreateMatchPage(match)));

			RoundListView.SelectedItem = null;
		}
	}
}