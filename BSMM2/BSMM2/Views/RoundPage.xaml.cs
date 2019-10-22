using BSMM2.Models;
using BSMM2.ViewModels;
using BSMM2.Views.Matches;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundPage : ContentPage {
		private RoundViewModel viewModel;

		public RoundPage() {
			InitializeComponent();

			BindingContext = viewModel = new RoundViewModel();
		}

		private async void OnMatchTapped(object sender, ItemTappedEventArgs args) {
			if (args.Item is Match match && viewModel.IsPlaying)
				await Navigation.PushModalAsync(new NavigationPage(new SingleMatchPage(viewModel.Game, match)));

			RoundListView.SelectedItem = null;
		}

		protected override void OnAppearing() {
			base.OnAppearing();

			if (viewModel.Matches.Count == 0)
				viewModel.LoadRoundCommand.Execute(null);
		}
	}
}