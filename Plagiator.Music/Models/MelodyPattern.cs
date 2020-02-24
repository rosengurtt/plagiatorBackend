using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plagiator.Music.Models
{

    public class MelodyPattern
    {
        public MelodyPattern() { }

        public MelodyPattern(PitchPattern pitchPattern, RythmPattern rythmPattern)
        {
            DeltaPitches = pitchPattern.DeltaPitches;
            RelativeDurations = rythmPattern.RelativeDurations;
        }
        public List<int> DeltaPitches { get; set; }

        public List<int> RelativeDurations { get; set; }

        public string AsString
        {
            get
            {
                return String.Join(",", DeltaPitches.Zip(RelativeDurations, (a, b) => $"({a}-{b})"));
            }
            set
            {
                DeltaPitches = new List<int>();
                RelativeDurations = new List<int>();
                string pattern = @"(\((\d*)\-(\d*)\))";
                MatchCollection matches = Regex.Matches(value, pattern);
                foreach (Match match in matches)
                {
                    GroupCollection data = match.Groups;
                    DeltaPitches.Add(int.Parse(data[2].Value));
                    RelativeDurations.Add(int.Parse(data[3].Value));
                    RelativeDurations = SimplifyDurations(RelativeDurations);
                }
                if (DeltaPitches.Count == 0 || RelativeDurations.Count == 0 ||
                    (DeltaPitches.Count < 2 && RelativeDurations.Count < 2))
                    throw new Exception("Invalid string for a melody pattern");
            }
        }


        public Pattern AsPattern
        {
            get
            {
                return new Pattern()
                {
                    AsString = this.AsString,
                    PatternTypeId = PatternType.Melody
                };
            }
        }
        private List<int> SimplifyDurations(List<int> durations)
        {
            var patty = new RythmPattern(durations);
            return patty.RelativeDurations;
        }
    }
}
