namespace BSMM2.Models {

	public interface IComparer {
		bool Active { get; set; }

		int Compare(Player p1, Player p2);
	}
}