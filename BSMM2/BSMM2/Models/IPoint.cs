namespace BSMM2.Models {

	public interface IPoint {//TODO : Will be SingleMatchPoint
		int MatchPoint { get; }

		int LifePoint { get; }

		double WinPoint { get; }
	}
}