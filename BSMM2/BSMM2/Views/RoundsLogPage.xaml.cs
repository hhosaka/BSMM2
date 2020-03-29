using BSMM2.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BSMM2.Views {

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoundsLogPage : TabbedPage {

		public RoundsLogPage(BSMMApp app) {
			InitializeComponent();
			int index = 1;
			foreach (var round in app.Game.Rounds) {
				Children.Add(CreatePage(new RoundLogPage(app.Game, round), "Round" + (index++)));
			}

			Page CreatePage(Page page, string title) {
				var ret = new NavigationPage(page);
				ret.Title = title;
				return ret;
			}
		}
	}
}