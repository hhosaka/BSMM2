using BSMM2.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BSMM2.ViewModels {

	internal class ResultConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var result = (IResult)value;
			return "Point=" + result.Point + "/Life=" + ToLifePoint(result.LifePoint) + "/Win=" + result.WinPoint;

			string ToLifePoint(int lifePoint)
				=> lifePoint >= 0 ? lifePoint.ToString() : "-";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}

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