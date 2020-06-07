namespace BSMM2.Models {

	public interface IPlayer : Exportable {
		string Name { get; }

		bool Dropped { get; }

		IPoint Point { get; }

		IPoint OpponentPoint { get; }

		bool HasByeMatch { get; }

		bool HasGapMatch { get; }

		string Information { get; }

		void Commit(Match match);
	}
}