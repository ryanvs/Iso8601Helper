using System;

namespace Iso8601Helper.Test
{
    public class DurationResult
    {
		public bool Result { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public TimeSpan? XmlSpan { get; set; }
		public TimeSpan? NewSpan { get; set; }
		public DateTimeOffset? XmlDate { get; set; }
		public DateTimeOffset? NewDate { get; set; }
		public IsoDuration Actual { get; set; }
		public IsoDuration Expected { get; set; }
		public string ErrorMessage { get; set; }
    }
}
