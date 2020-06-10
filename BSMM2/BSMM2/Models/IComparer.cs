namespace BSMM2.Models {

	internal interface IComparer {
		string Name { get; }
		string Description { get; }
		bool Active { get; set; }

		int Compare(Player p1, Player p2);
	}
}