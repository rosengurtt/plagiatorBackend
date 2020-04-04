using System;
using System.Collections.Generic;
using System.Linq;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;

namespace Plagiator.Analysis.Patterns
{

    public class PitchPattern
    {
        public PitchPattern(Pattern pattern)
        {
            if (pattern.PatternTypeId != PatternType.Pitch)
                throw new Exception("Can't create a PitchPattern object from this PatternType");
            AsString = pattern.AsString;
        }
        public PitchPattern() { }

        public List<int> DeltaPitches { get; set; }


        public string AsString
        {
            get
            {
                return String.Join(",", DeltaPitches);
            }
            set
            {
                DeltaPitches = Array
                    .ConvertAll(value.Split(","), s => int.Parse(s)).ToList();
            }
        }
        public int Length
        {
            get
            {
                return DeltaPitches.Count;
            }
        }

        public List<MelodyPattern> MelodyPatterns { get; set; }

        private List<int> PitchesRelativeToFirst
        {
            get
            {
                var pitchesRelativeToFirst = new List<int>();
                pitchesRelativeToFirst.Add(0);
                for (int i = 1; i < DeltaPitches.Count; i++)
                {
                    var pitch = DeltaPitches[i] + pitchesRelativeToFirst[i - 1];
                    pitchesRelativeToFirst.Add(pitch);
                }
                return pitchesRelativeToFirst;
            }
        }
        public List<int> intervals
        {
            get
            {
                var retObj = new List<int>() { 0 };
                for (int i = 0; i < PitchesRelativeToFirst.Count - 1; i++)
                {
                    for (int j = i + 1; j < PitchesRelativeToFirst.Count; j++)
                    {
                        if (!retObj.Contains((j - i + 96) % 12))
                            retObj.Add((j - i + 96) % 12);
                    }
                }
                return retObj;
            }
        }
    }
}

