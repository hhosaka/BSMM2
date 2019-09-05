using Newtonsoft.Json;

namespace BSMM2.Models.Rules.Match {

	[JsonObject]
	public interface IMatchResult : IResult {
		int MatchPoint { get; }

		int LifePoint { get; }

		double WinPoint { get; }
	}
}