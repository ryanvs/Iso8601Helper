using System;

namespace Iso8601Helper.Test
{
	public struct DurationTest
	{
		public string Input { get; set; }
		public IsoDuration Expected { get; set; }
		public IFormatProvider Provider { get; set; }
	}
}
