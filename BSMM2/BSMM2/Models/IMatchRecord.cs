namespace BSMM2.Models {

	public interface IMatchRecord {
		IPlayer Player { get; }

		RESULT_T Result { get; }

		bool IsFinished { get; }

		IPoint Point { get; }
	}
}