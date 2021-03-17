using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Iso8601Helper.Test
{
    [TestClass]
	public class IsoIntervalTests
    {
        [TestMethod]
        public void TestIntervals()
        {
			string[] inputs = new string[]
			{
				"2007-03-01T13:00:00Z/2008-05-11T15:30:00Z",
				"2007-03-01T13:00:00Z/P1Y2M10DT2H30M",
				"P1Y2M10DT2H30M/2008-05-11T15:30:00Z",
				"P1Y2M10DT2H30M",
			};

			foreach (var input in inputs)
			{
				IsoTimeInterval interval;
				bool result = IsoTimeInterval.TryParse(input, out interval);
				if (!result)
					Assert.Fail("IsoTimeInterval.TryParse failed: {0}", input);
				Console.WriteLine(interval);
			}
		}
	}
}
