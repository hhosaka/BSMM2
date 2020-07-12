namespace BSMM2.Models {

	public interface IPoint {
		int MatchPoint { get; }

		double WinPoint { get; }

		int? LifePoint { get; }
	}
}