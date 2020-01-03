using System.Collections.Generic;
using System.Linq;

namespace BSMM2.Models {

	internal class DefaultGame : Game {

		public override IEnumerable<Player> PlayersByOrder
			=> Enumerable.Empty<Player>();
	}
}