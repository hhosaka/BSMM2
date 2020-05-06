using BSMM2.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RulePage : ContentPage {

		public RulePage(BSMMApp app) {
			InitializeComponent();
			BindingContext = app.Game.Rule;
		}

		private async void Back(object sender, EventArgs args)
			=> await Navigation.PopModalAsync();
	}
}