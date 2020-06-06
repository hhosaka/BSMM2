namespace BSMM2.Models {

	public interface IMatchRecord {
		IPlayer Player { get; }

		//IResult Result { get; }
		RESULT_T Result { get; }

		bool IsFinished { get; }

		IPoint Point { get; }
	}
}