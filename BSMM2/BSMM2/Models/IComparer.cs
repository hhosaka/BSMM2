namespace BSMM2.Models {

	public interface IComparer {
		string Label { get; }

		bool Mandatory { get; }

		bool Active { get; set; }

		int Compare(Player p1, Player p2);
	}
}