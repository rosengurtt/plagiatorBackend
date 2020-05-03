
using NUnit.Framework;
using Plagiator.Analysis.Patterns;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;
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


        [Test]
        public void Simplification4works()
        {
            var paton = new Pattern
            {
                AsString = "1,1,1,1,1",
                PatternTypeId = PatternType.Pitch
            };
            var simplito = PatternUtilities.Simpl4(paton);
            Assert.AreEqual(simplito.Length, 1);

            var patin = new Pattern
            {
                AsString = "2,1,2,1,2,1",
                PatternTypeId = PatternType.Pitch
            };
            var simplote = PatternUtilities.Simpl4(patin);
            Assert.AreEqual(simplote.Length, 2);

            var pate = new Pattern
            {
                AsString = "2,1,5,4,3,7,3,1,2,6,7,8,9",
                PatternTypeId = PatternType.Pitch
            };
            var simplin = PatternUtilities.Simpl4(pate);
            Assert.AreEqual(simplin.Length, 13);
        }
    }
}