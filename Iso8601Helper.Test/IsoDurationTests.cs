using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Xml;

namespace Iso8601Helper.Test
{
    [TestClass]
    public class IsoDurationTests
    {
		public DateTimeOffset BaseDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

		protected DurationResult TestInput(DurationTest test)
        {
			IsoDuration d;
			string errorMessage;
			var result = IsoDuration.TryParse(test.Input, test.Provider, out d, out errorMessage);

			DateTimeOffset? xmlDate = null;
			TimeSpan? xmlSpan = null;
			try
			{
				xmlSpan = XmlConvert.ToTimeSpan(test.Input);
				xmlDate = BaseDate + xmlSpan.Value;
			}
			catch
			{
				xmlDate = null;
			}

			DateTimeOffset? newDate = null;
			TimeSpan? newSpan = null;

			try
			{
				newDate = d.Add(BaseDate);
				newSpan = newDate - BaseDate;
			}
			catch
			{
				newSpan = null;
			}

			var x = new DurationResult
			{
				Result = result,
				Input = test.Input,
				Output = Convert.ToString(d, test.Provider),
				XmlSpan = xmlSpan,
				NewSpan = newSpan,
				XmlDate = xmlDate,
				NewDate = newDate,
				Actual = d,
				Expected = test.Expected,
				ErrorMessage = errorMessage,
			};

			if (!result)
			{
				Assert.Fail("IsoDuration.TryParse should succeed: Input={0}", test.Input);
			}
			if (x.Input != x.Output)
			{
				Assert.Fail("Input does not match Output: Input={0}, Output={1}", test.Input, x.Output);
			}

			var e = test.Expected;
			if (d.IsNegative != e.IsNegative) { Assert.Fail(nameof(e.IsNegative)); }
			if (d.Years != e.Years) { Assert.Fail(nameof(e.Years)); }
			if (d.Months != e.Months) { Assert.Fail(nameof(e.Months)); }
			if (d.Weeks != e.Weeks) { Assert.Fail(nameof(e.Weeks)); }
			if (d.Days != e.Days) { Assert.Fail(nameof(e.Days)); }
			if (d.Hours != e.Hours) { Assert.Fail(nameof(e.Hours)); }
			if (d.Minutes != e.Minutes) { Assert.Fail(nameof(e.Minutes)); }
			if (d.Seconds != e.Seconds) { Assert.Fail(nameof(e.Seconds)); }

			return x;
		}

		[TestMethod]
        public void TestDurationYears()
        {
			var tests = new DurationTest[]
			{
				new DurationTest { Input = "-P3Y6M4DT12H30M5.123S"  , Expected = new IsoDuration { IsNegative = true, Years = -3, Months = -6, Days = -4, Hours = -12, Minutes = -30, Seconds = -5.123m } }
			,   new DurationTest { Input = "P3Y6M4DT12H30M5S"       , Expected = new IsoDuration { Years = 3, Months = 6, Days = 4, Hours = 12, Minutes = 30, Seconds = 5 } }
			,   new DurationTest { Input = "-P2W"                   , Expected = new IsoDuration { IsNegative = true, Weeks = -2 } }
			,   new DurationTest { Input = "P23DT23H"               , Expected = new IsoDuration { Days = 23, Hours = 23 } }
			,   new DurationTest { Input = "P4Y"                    , Expected = new IsoDuration { Years = 4 } }
			,   new DurationTest { Input = "P1M"                    , Expected = new IsoDuration { Months = 1 } }
			,   new DurationTest { Input = "PT1M"                   , Expected = new IsoDuration { Minutes = 1 } }
			,   new DurationTest { Input = "P0.5Y"                  , Expected = new IsoDuration { Years = 0.5m } }
			,   new DurationTest { Input = "P0.6Y"                  , Expected = new IsoDuration { Years = 0.6m } }
			,   new DurationTest { Input = "P0.75Y"                 , Expected = new IsoDuration { Years = 0.75m } }
			,   new DurationTest { Input = "P0.3M"                  , Expected = new IsoDuration { Months = 0.3m } }
			,   new DurationTest { Input = "PT36H"                  , Expected = new IsoDuration { Hours = 36 } }
			,   new DurationTest { Input = "P1DT12H"                , Expected = new IsoDuration { Days = 1, Hours = 12 } }
			,   new DurationTest { Input = "-P2W"                   , Expected = new IsoDuration { IsNegative = true, Weeks = -2 } }
			,   new DurationTest { Input = "-P2.2W"                 , Expected = new IsoDuration { IsNegative = true, Weeks = -2.2m } }
			,   new DurationTest { Input = "P1DT2H3M4S"             , Expected = new IsoDuration { Days = 1, Hours = 2, Minutes = 3, Seconds = 4 } }
			,   new DurationTest { Input = "P1DT2H3M"               , Expected = new IsoDuration { Days = 1, Hours = 2, Minutes = 3 } }
			,   new DurationTest { Input = "P1DT2H"                 , Expected = new IsoDuration { Days = 1, Hours = 2 } }
			,   new DurationTest { Input = "PT2H"                   , Expected = new IsoDuration { Hours = 2 } }
			,   new DurationTest { Input = "PT2.3H"                 , Expected = new IsoDuration { Hours = 2.3m } }
			,   new DurationTest { Input = "PT2H3M4S"               , Expected = new IsoDuration { Hours = 2, Minutes = 3, Seconds = 4 } }
			,   new DurationTest { Input = "PT3M4S"                 , Expected = new IsoDuration { Minutes = 3, Seconds = 4 } }
			,   new DurationTest { Input = "PT22S"                  , Expected = new IsoDuration { Seconds = 22 } }
			,   new DurationTest { Input = "PT22.22S"               , Expected = new IsoDuration { Seconds = 22.22m } }
			,   new DurationTest { Input = "-P2Y"                   , Expected = new IsoDuration { IsNegative = true, Years = -2 } }
			,   new DurationTest { Input = "-P1DT2H3M4S"            , Expected = new IsoDuration { IsNegative = true, Days = -1, Hours = -2, Minutes = -3, Seconds = -4 } }
			// French - using comma instead of decimal point
			,   new DurationTest { Input = "PT12,345S"              , Expected = new IsoDuration { Seconds = 12.345m }, Provider = new CultureInfo("fr-FR") }
			};

            foreach (var test in tests)
            {
				TestInput(test);
			}
        }

		[TestMethod]
		public void TestFailures()
        {
			var failTests = new[]
			{
				"P1.1YT2.5H",		// Can only use demical in final period
				"-P1.1YT2.5H",		// Can only use demical in final period
				"P100,000.123D",	// Can only use either demical or comma, but not both
				"P100.000,123D",	// Can only use either demical or comma, but not both
			};

			foreach (var test in failTests)
			{
				IsoDuration d;
				string errorMessage;
				var result = IsoDuration.TryParse(test, out d, out errorMessage);
				if (result != false) { Assert.Fail("IsoDuration.TryParse should fail"); }
			}
		}
	}
}
