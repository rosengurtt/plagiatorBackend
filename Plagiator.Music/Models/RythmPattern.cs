using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents the relative lenghts between the start of the notes
    /// If all lengths are equal, then RithmPattern = [1]
    /// If the lengths are quarter, eight, eight, quarter, quarter,
    /// then RythmPattern is [2,1,2,2]
    /// </summary>
    public class RythmPattern
    {

        public RythmPattern() { }

        public RythmPattern(Pattern pattern) {
            AsString = pattern.AsString;
        }
        [NotMapped]
        private List<int> relativeDurations;

        [NotMapped]
        public List<int> RelativeDurations
        {
            get
            {
                return relativeDurations;
            }
            set
            {
                // Simplify cases like 17,15,16 to 1,1,1
                if (value.Min() > 4 && (value.Max() - value.Min() <= 2))
                {
                    relativeDurations = new List<int>() { 1 };
                    return;
                }
                // Divide by the maximum common divisor, so for ex 4,2,2 is converted to 2,1,1
                relativeDurations = new List<int>();
                int gcd = GCD(value.ToArray());
                for (int i = 0; i < value.Count; i++)
                    relativeDurations.Add(value[i] / gcd);
                // If the pattern itself has a pattern, get the shortest pattern
                // For ex instead of 2,1,2,1,2,1 we want just 2,1
                relativeDurations = PatternUtilities.GetShortestPattern(relativeDurations);
            }
        }

        public string AsString
        {
            get
            {
                return  String.Join(",", RelativeDurations);
            }
            set
            {
                RelativeDurations = Array.ConvertAll(value.Split(","), s => int.Parse(s)).ToList();
            }
        }

        [NotMapped]
        public int Length
        {
            get
            {
                return relativeDurations.Count;
            }
        }

        public List<MelodyPattern> MelodyPatterns { get; set; }
        private static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        private static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
     
       
    }
}
