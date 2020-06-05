namespace BSMM2.Models {

	public interface IResult : Exportable {
		RESULT_T RESULT { get; }

		bool IsFinished { get; }

		string Information { get; }

		IPoint GetPoint();
	}
}