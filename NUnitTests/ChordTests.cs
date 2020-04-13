using NUnit.Framework;
using Plagiator.Analysis.Patterns;
using Plagiator.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NUnitTests
{
    class ChordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ExtractIntervalsFromChords() {
            var chordito = new Chord("47,55,55,55,59,62,67,74");
            Assert.AreEqual(chordito.Intervals, "0,3,4,5,7,8");
        }

        public void ExtractPitchesAsLetters()
        {
            var chordito = new Chord("47,55,55,55,59,62,67,74");
            Assert.AreEqual(chordito.PitchLettersAsString, "B,D,G");
        }
    }
}
