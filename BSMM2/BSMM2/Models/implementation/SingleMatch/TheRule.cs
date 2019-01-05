using BSMM2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace BSMM2.Modules.Rules.SingleMatch {

	[JsonObject]
	public class TheRule : Rule {

		public override IEnumerable<Func<object, object, int>> Compareres
			=> Result.Compareres;

		public override ContentPage ContentPage
			=> null;
	}
}