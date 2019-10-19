using BSMM2.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	internal class ResultConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is IResult result)
				return "Point=" + result.Point + "/Life=" + result.LifePoint + "/Win=" + result.WinPoint;
			else
				return "***";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}

	public class RoundResultConverter : IValueConverter {
		private static int i = 0;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return "(" + GetResult() + ")";

			string GetResult() {
				var r = (value as IMatchRecord)?.Result?.RESULT;
				if (r != null)
					return r.ToString();
				else
					return "I=" + (++i);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}