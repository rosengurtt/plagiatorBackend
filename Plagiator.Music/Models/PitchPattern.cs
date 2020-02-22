using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System;

namespace Plagiator.Music.Models
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

        private List<int> pitchesRelativeToFirst { get; set; }

        public List<int> PitchesRelativeToFirst {
            get
            {
                return pitchesRelativeToFirst;
            }
            set
            {
                pitchesRelativeToFirst = PatternUtilities.GetShortestPattern(value);
            }
        }
        public string AsString
        {
            get
            {
                return String.Join(",", PitchesRelativeToFirst);
            }
            set
            {
                PitchesRelativeToFirst = Array
                    .ConvertAll(value.Split(","), s => int.Parse(s)).ToList();
            }
        }
        public int Length
        {
            get
            {
                return PitchesRelativeToFirst.Count + 1;
            }
        }

        public List<MelodyPattern> MelodyPatterns { get; set; }

        public List<int> intervals
        {
            get
            {
                var retObj = new List<int>();
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
