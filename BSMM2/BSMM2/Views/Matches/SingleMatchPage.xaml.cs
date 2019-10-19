using BSMM2.Models;
using BSMM2.ViewModels.Matches;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views.Matches {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SingleMatchPage : ContentPage {
		private SingleMatchViewModel _viewModel;

		public SingleMatchPage(Game game, Match match) {
			InitializeComponent();
			BindingContext = _viewModel = new SingleMatchViewModel(game, match);
			_viewModel.Popup += async () => await Navigation.PopModalAsync();
		}
	}
}