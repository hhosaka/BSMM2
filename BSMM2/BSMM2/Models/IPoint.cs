namespace BSMM2.Models {

	public interface IPoint : Exportable {
		int Point { get; }

		int LifePoint { get; }

		double WinPoint { get; }

		string Information { get; }

		int? CompareTo(IPoint point, int strictness = 0);
	}
}