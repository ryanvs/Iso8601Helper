using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Iso8601Helper
{
	// https://en.wikipedia.org/wiki/ISO_8601#Time_intervals
	public class IsoTimeInterval
	{
		public const string PrimarySolidus = "/";
		public const string SecondarySolidus = "--";

		public string Input { get; set; }
		public string[] Sections { get; private set; }
		public string Solidus { get; set; } = PrimarySolidus;
		public IsoTimeIntervalKind Kind { get; set; }
		public DateTimeOffset? Start { get; set; }
		public DateTimeOffset? End { get; set; }
		public TimeSpan? Duration { get; set; }

		private enum SectionKind
		{
			Invalid,
			DateTime,
			Duration,
		}

		private static bool TryDuration(string input, out TimeSpan duration)
		{
			try
			{
				duration = XmlConvert.ToTimeSpan(input);
				return true;
			}
			catch
			{
				duration = default(TimeSpan);
				return false;
			}
		}

		public static bool TryParse(string input, out IsoTimeInterval interval)
		{
			if (string.IsNullOrWhiteSpace(input))
				throw new ArgumentNullException(nameof(input));

			bool result = false;
			string sep = PrimarySolidus;
			if (input.IndexOf(sep) == -1)
			{
				sep = SecondarySolidus;
			}
			interval = new IsoTimeInterval() { Input = input };
			interval.Sections = input.Split(new string[] { sep }, StringSplitOptions.None);
			if (interval.Sections.Length > 2)
				throw new ArgumentException($"Too many Solidus '{sep}' separators. Only one solidus is allowed.", nameof(input));

			if (interval.Sections.Length == 1)
			{
				sep = SecondarySolidus;
				interval.Sections = input.Split(new string[] { sep }, StringSplitOptions.None);
				if (interval.Sections.Length > 2)
					throw new ArgumentException($"Too many Solidus '{sep}' separators. Only one solidus is allowed.", nameof(input));
				else if (interval.Sections.Length == 2)
					interval.Solidus = sep;
			}
			if (interval.Sections.Length == 1)
			{
				// Check for duration only
				TimeSpan ts;
				if (TryDuration(interval.Sections[0], out ts))
				{
					interval.Kind = IsoTimeIntervalKind.Duration;
					interval.Duration = ts;
					result = true;
				}
			}
			else //if (interval.Sections.Length == 2)
			{
				var sectionKinds = new SectionKind[2];
				for (int index = 0; index < 2; ++index)
				{
					DateTimeOffset dto;
					TimeSpan ts;

					if (DateTimeOffset.TryParse(interval.Sections[index], out dto))
					{
						sectionKinds[index] = SectionKind.DateTime;
						if (index == 0)
							interval.Start = dto;
						else if (index == 1)
							interval.End = dto;
					}
					else if (TryDuration(interval.Sections[index], out ts))
					{
						sectionKinds[index] = SectionKind.Duration;
						interval.Duration = ts;
					}
				}

				if (sectionKinds[0] == SectionKind.DateTime && sectionKinds[1] == SectionKind.DateTime)
					interval.Kind = IsoTimeIntervalKind.StartEnd;
				else if (sectionKinds[0] == SectionKind.DateTime && sectionKinds[1] == SectionKind.Duration)
					interval.Kind = IsoTimeIntervalKind.StartDuration;
				else if (sectionKinds[0] == SectionKind.Duration && sectionKinds[1] == SectionKind.DateTime)
					interval.Kind = IsoTimeIntervalKind.DurationEnd;
				else
					throw new ArgumentException($"Invalid TimeInterval Input: 1={sectionKinds[0]}, 2={sectionKinds[1]}", nameof(input));

				result = true;
			}

			return result;
		}
	}
}
