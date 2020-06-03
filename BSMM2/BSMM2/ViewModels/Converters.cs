using BSMM2.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	public class RoundResultConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var record = (value as IMatchRecord);
			if (parameter is "BGCOLOR") {
				return record.Result.IsFinished ? Color.Aqua : Color.White;
			} else {
				return "(" + GetResult() + ")";
			}

			string GetResult()
				=> (value as IMatchRecord).Result.RESULT.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}