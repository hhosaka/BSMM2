namespace BSMM2.Models {

	public interface IPlayer : Exportable {
		string Name { get; }

		bool Dropped { get; }

		IResult Result { get; }

		IResult OpponentResult { get; }

		bool HasByeMatch { get; }

		bool HasGapMatch { get; }

		void Commit(Match match);
	}
}