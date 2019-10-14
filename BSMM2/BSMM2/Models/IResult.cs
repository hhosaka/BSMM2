namespace BSMM2.Models {

	public interface IResult {
		RESULT_T? RESULT { get; }

		int Point { get; }

		int LifePoint { get; }

		double WinPoint { get; }

		bool IsFinished { get; }
	}
}