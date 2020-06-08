namespace BSMM2.Models {

	public interface IPoint {//TODO : Will be SingleMatchPoint
		int Point { get; }

		int LifePoint { get; }

		double WinPoint { get; }
	}
}