namespace Iso8601Helper
{
	public enum IsoTimeIntervalKind
	{
		Invalid,
		DateTime,       // DateTime only (no solidus)  == 2018-01-01Z
		Start,          // Start only (solidus at end) == 2018-01-01Z/
		End,            // End only (solidus at start) == /2018-01-01Z
		StartEnd,       // Start and End               == 2018-01-01Z/2018-01-02Z
		StartDuration,  // Start and Duration          == 2018-01-01Z/P1DT12H30M
		DurationEnd,    // Duration and End            == P1DT12H30M/2018-01-01Z
		Duration,       // Duration only (no solidus)  == P1DT12H30M
	}
}
