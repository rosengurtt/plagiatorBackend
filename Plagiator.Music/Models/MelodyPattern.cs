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
            PitchesRelativeToFirst = pitchPattern.PitchesRelativeToFirst;
            RelativeDurations = rythmPattern.RelativeDurations;
        }
        public List<int> PitchesRelativeToFirst { get; set; }

        public List<int> RelativeDurations { get; set; }

        public string AsString
        {
            get
            {
                return String.Join(",", PitchesRelativeToFirst.Zip(RelativeDurations, (a, b) => $"({a}-{b}"));
            }
            set
            {
                PitchesRelativeToFirst = new List<int>();
                RelativeDurations = new List<int>();
                string pattern = @"(\((\d*)\-(\d*)\))";
                MatchCollection matches = Regex.Matches(value, pattern);
                foreach (Match match in matches)
                {
                    GroupCollection data = match.Groups;
                    PitchesRelativeToFirst.Add(int.Parse(data[2].Value));
                    RelativeDurations.Add(int.Parse(data[3].Value));
                    RelativeDurations = SimplifyDurations(RelativeDurations);
                }
                if (PitchesRelativeToFirst.Count == 0 || RelativeDurations.Count == 0 ||
                    (PitchesRelativeToFirst.Count < 2 && RelativeDurations.Count < 2))
                    throw new Exception("Invalid string for a melody pattern");
            }
        }
       
        public Pattern AsPattern
        {
            get
            {
                return new Pattern()
                {
                    AsString = this.AsString
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
