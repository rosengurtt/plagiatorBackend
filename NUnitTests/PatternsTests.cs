
using NUnit.Framework;
using Plagiator.Music;
using System.Collections.Generic;
using System.Linq;

namespace NUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FindPatternsInListOfIntegers_FindsLongestPatternsOnly()
        {
            var listita = new List<int> { 3, 3, 4, 5, 7, 8, 3, 5, 4, 3, 3, 5, 3, 3, 4, 5, 2, 1, 3, 4 };
            var patterns = PatternUtilities.FindPatternsInListOfIntegers(listita,3,10).ToList();
            Assert.AreEqual(patterns.Count, 1);
        }
    }
}