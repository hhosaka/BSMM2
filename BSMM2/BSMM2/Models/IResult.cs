namespace BSMM2.Models {

	public interface IResult : IPoint, Exportable {
		RESULT_T RESULT { get; }

		bool IsFinished { get; }

		string Information { get; }
	}
}