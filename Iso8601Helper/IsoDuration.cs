using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Iso8601Helper
{
	public class IsoDuration : IFormattable
	{
		public static readonly Regex IsoRegex = new Regex("^(-)?P(?:([0-9,.]*)Y)?(?:([0-9,.]*)M)?(?:([0-9,.]*)W)?(?:([0-9,.]*)D)?(?:T(?:([0-9,.]*)H)?(?:([0-9,.]*)M)?(?:([0-9,.]*)S)?)?$");

		public IsoDuration()
		{ }

		public IsoDuration(decimal years = 0, decimal months = 0, decimal weeks = 0, decimal days = 0, decimal hours = 0, decimal minutes = 0, decimal seconds = 0)
		{
			Years = years;
			Months = months;
			Weeks = weeks;
			Days = days;
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
			IsNegative = years < 0 || months < 0 || weeks < 0 || days < 0 || hours < 0 || minutes < 0 || seconds < 0;
		}

		public bool IsNegative { get; set; }
		public decimal Years { get; set; }
		public decimal Months { get; set; }
		public decimal Weeks { get; set; }
		public decimal Days { get; set; }
		public decimal Hours { get; set; }
		public decimal Minutes { get; set; }
		public decimal Seconds { get; set; }

		/// <summary>
		/// Helper class to parse each field or component in a duration
		/// </summary>
		public class ParseField
		{
			public bool IsNegative { get; set; }
			public bool HasComma { get; set; }
			public bool HasDecimal { get; set; }

			public bool? Parse(string input, Action<decimal> setProp)
			{
				return Parse(input, CultureInfo.CurrentCulture, setProp);
			}

			public bool? Parse(string input, IFormatProvider provider, Action<decimal> setProp)
			{
				string errorMessage = null;
				var result = TryParse(input, provider, setProp, out errorMessage);
				if (errorMessage != null)
					throw new FormatException(errorMessage);
				return result;
			}

			public bool? TryParse(string input, Action<decimal> setProp)
			{
				string errorMessage;
				return TryParse(input, CultureInfo.CurrentCulture, setProp, out errorMessage);
			}

			public bool? TryParse(string input, IFormatProvider provider, Action<decimal> setProp)
			{
				string errorMessage;
				return TryParse(input, provider, setProp, out errorMessage);
			}

			public bool? TryParse(string input, Action<decimal> setProp, out string errorMessage)
			{
				return TryParse(input, CultureInfo.CurrentCulture, setProp, out errorMessage);
			}

			/// <summary>
			/// Parses the numeric portion of a specific component of an IsoDuration
			/// </summary>
			/// <returns>True if the numeric portion exists and is successfully parsed. False </returns>
			public bool? TryParse(string input, IFormatProvider provider, Action<decimal> setProp, out string errorMessage)
			{
				bool? result = null;
				errorMessage = null;
				if (!string.IsNullOrEmpty(input))
				{
					bool hasComma = input.IndexOf(',') >= 0;
					bool hasDecimal = input.IndexOf('.') >= 0;

					if ((hasComma || hasDecimal) && (HasComma || HasDecimal))
					{
						errorMessage = "Decimal fraction can only be specified in last component";
						return false;
					}
					if (hasComma && hasDecimal)
					{
						errorMessage = "Cannot have both decimal and comma in number. Use either decimal or comma but not both.";
						return false;
					}

					HasComma = hasComma;
					HasDecimal = hasDecimal;

					decimal value;
					result = decimal.TryParse(input, NumberStyles.AllowDecimalPoint, provider, out value);
					if (result ?? false)
					{
						if (IsNegative)
							value = -value;
						setProp(value);
					}
					else
					{
						errorMessage = "Failed to parse component: " + input;
						return false;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Are all components zero?
		/// </summary>
		public bool IsEmpty
		{
			get { return (Years == 0 && Months == 0 && Weeks == 0 && Days == 0 && Hours == 0 && Minutes == 0 && Seconds == 0); }
		}


		/// <summary>
		/// Does the duration represent an ambiguous or inexact time period that may change depending on the starting date and time?
		/// The duration may vary due to leap year, days in a month, or daylight savings change.
		/// </summary>
		public bool IsAmbiguous
		{
			get { return (Years != 0 || Months != 0 || Weeks != 0 || Days != 0); }
		}

		/// <summary>
		/// Does the duration represent an exact time period that does not change?
		/// </summary>
		public bool IsExact
		{
			get { return !IsAmbiguous; }
		}

		/// <summary>
		/// Does the duration have any Date components (years, months, weeks, or days)?
		/// </summary>
		public bool HasDate
		{
			get { return (Years != 0 || Months != 0 || Weeks != 0 || Days != 0); }
		}

		/// <summary>
		/// Does the duration have any Time components (hours, minutes, or seconds)?
		/// </summary>
		public bool HasTime
		{
			get { return (Hours != 0 || Minutes != 0 || Seconds != 0); }
		}

		public static IsoDuration Parse(string input)
		{
			return Parse(input, CultureInfo.CurrentCulture);
		}

		public static IsoDuration Parse(string input, IFormatProvider provider)
		{
			string errorMessage;
			IsoDuration result;
			TryParse(input, provider, out result, out errorMessage);
			if (errorMessage != null)
				throw new FormatException(errorMessage);
			return result;
		}

		public static bool TryParse(string input, out IsoDuration result)
		{
			string errorMessage;
			return TryParse(input, CultureInfo.CurrentCulture, out result, out errorMessage);
		}

		public static bool TryParse(string input, IFormatProvider provider, out IsoDuration result)
		{
			string errorMessage;
			return TryParse(input, provider, out result, out errorMessage);
		}

		public static bool TryParse(string input, out IsoDuration result, out string errorMessage)
		{
			return TryParse(input, CultureInfo.CurrentCulture, out result, out errorMessage);
		}

		public static bool TryParse(string input, IFormatProvider provider, out IsoDuration result, out string errorMessage)
		{
			errorMessage = null;
			var match = IsoRegex.Match(input);
			bool success = match.Success;
			if (success)
			{
				var duration = new IsoDuration();

				bool negative = match.Groups[1].Success;
				duration.IsNegative = negative;

				var parser = new ParseField() { IsNegative = negative };
				if (errorMessage == null) { parser.TryParse(match.Groups[2].Value, provider, x => { duration.Years = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[3].Value, provider, x => { duration.Months = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[4].Value, provider, x => { duration.Weeks = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[5].Value, provider, x => { duration.Days = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[6].Value, provider, x => { duration.Hours = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[7].Value, provider, x => { duration.Minutes = x; }, out errorMessage); }
				if (errorMessage == null) { parser.TryParse(match.Groups[8].Value, provider, x => { duration.Seconds = x; }, out errorMessage); }
				if (errorMessage == null)
				{
					result = duration;
					success = true;
				}
				else
				{
					result = default(IsoDuration);
					success = false;
				}
			}
			else
			{
				result = default(IsoDuration);
			}
			return success;
		}

		public override string ToString()
		{
			return ToString(CultureInfo.CurrentCulture);
		}

		string IFormattable.ToString(string format, IFormatProvider provider)
		{
			return ToString(provider);
		}

		public string ToString(IFormatProvider provider)
		{
			var builder = new StringBuilder();

			if (IsNegative) builder.Append("-");
			builder.Append("P");
			if (Years != 0) { builder.AppendFormat(provider, "{0}Y", Math.Abs(Years)); }
			if (Months != 0) { builder.AppendFormat(provider, "{0}M", Math.Abs(Months)); }
			if (Weeks != 0) { builder.AppendFormat(provider, "{0}W", Math.Abs(Weeks)); }
			if (Days != 0) { builder.AppendFormat(provider, "{0}D", Math.Abs(Days)); }

			if (Hours != 0 || Minutes != 0 || Seconds != 0)
			{
				builder.Append("T");
				if (Hours != 0) { builder.AppendFormat(provider, "{0}H", Math.Abs(Hours)); }
				if (Minutes != 0) { builder.AppendFormat(provider, "{0}M", Math.Abs(Minutes)); }
				if (Seconds != 0) { builder.AppendFormat(provider, "{0}S", Math.Abs(Seconds)); }
			}

			return builder.ToString();
		}

		/* ====
		public static IsoDuration FromInterval(DateTime start, DateTime end)
		{
		}

		public static IsoDuration FromInterval(DateTimeOffset start, DateTimeOffset end)
		{
		}
		==== */

		protected DateTimeOffset AddField(DateTimeOffset dt, IsoDurationField component, decimal value)
		{
			var dValue = (double)value;
			int whole = (int)Math.Truncate(value);
			decimal fraction = value - Math.Truncate(value);

			switch (component)
			{
				case IsoDurationField.Years:
					dt = dt.AddYears(whole);
					if (fraction != 0)
					{
						// Fraction is the percentage of days in the ending year
						var days = DateTime.IsLeapYear(dt.Year) ? 366 : 365;
						dValue = (double)(fraction * days);
						return dt.AddDays(dValue);
					}
					return dt;
				case IsoDurationField.Months:
					dt = dt.AddMonths(whole);
					if (fraction != 0)
					{
						// Fraction is the percentage of days in the ending month
						var days = DateTime.DaysInMonth(dt.Year, dt.Month);
						dValue = (double)(fraction * days);
						return dt.AddDays(dValue);
					}
					return dt;
				case IsoDurationField.Weeks: { return dt.AddDays(dValue * 7); }
				case IsoDurationField.Days: { return dt.AddDays(dValue); }
				case IsoDurationField.Hours: { return dt.AddHours(dValue); }
				case IsoDurationField.Minutes: { return dt.AddMinutes(dValue); }
				case IsoDurationField.Seconds: { return dt.AddSeconds(dValue); }
				default: return dt;
			}
		}

		public DateTimeOffset Add(DateTimeOffset value)
		{
			var result = new DateTimeOffset(value.Ticks, value.Offset);
			if (Years != 0) { result = AddField(result, IsoDurationField.Years, Years); }
			if (Months != 0) { result = AddField(result, IsoDurationField.Months, Months); }
			if (Weeks != 0) { result = AddField(result, IsoDurationField.Weeks, Weeks); }
			if (Days != 0) { result = AddField(result, IsoDurationField.Days, Days); }
			if (Hours != 0) { result = AddField(result, IsoDurationField.Hours, Hours); }
			if (Minutes != 0) { result = AddField(result, IsoDurationField.Minutes, Minutes); }
			if (Seconds != 0) { result = AddField(result, IsoDurationField.Seconds, Seconds); }
			return result;
		}

		public DateTime Add(DateTime value)
		{
			var result = new DateTimeOffset(value);
			result = Add(value);
			return result.DateTime;
		}

		/* ====
		public TimeSpan Subtract(DateTime value)
		{
		}

		public TimeSpan Subtract(DateTimeOffset value)
		{
		}
		==== */

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}
}
