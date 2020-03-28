﻿using BSMM2.Models;
using BSMM2.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayerPage : ContentPage {

		public PlayerPage(BSMMApp app, Player player) {
			InitializeComponent();
			BindingContext = new PlayerViewModel(app, player);
		}

		private void OnClosing(object sender, EventArgs e)
			=> MessagingCenter.Send<object>(this, Messages.REFRESH);
	}
}