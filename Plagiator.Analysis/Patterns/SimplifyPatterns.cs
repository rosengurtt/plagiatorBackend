using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis.Patterns
{
    public static partial class PatternUtilities
    {
        // Simplify cases like 17,15,16 to 1,1,1
        private static Pattern Simpl1(Pattern pattern)
        {
            if (pattern.PatternTypeId == PatternType.Pitch) return pattern;

            List<int> RelativeDurations = GetDurationsOfPattern(pattern);

            if (RelativeDurations.Min() > 4 && (RelativeDurations.Max() - RelativeDurations.Min() <= 2))
            {
                return ChangePatternDurations(pattern, new List<int> { 1 });
            }
            return pattern;
        }
        // Simplify cases like 16,3,3,3,3,6
        private static Pattern Simpl2(Pattern pattern)
        {
            if (pattern.PatternTypeId == PatternType.Pitch) return pattern;

            List<int> RelativeDurations = GetDurationsOfPattern(pattern);

            var min = RelativeDurations.Min();
            var totalWithPerfectProportion = 0;
            foreach (var d in RelativeDurations)
            {
                if (d % min == 0) totalWithPerfectProportion++;
            }
            if (pattern.Length - totalWithPerfectProportion == 1 ||
                totalWithPerfectProportion / (double)pattern.Length > 0.8)
            {
                var newDurations = RelativeDurations
                    .Select(x => x % min == 0 ? x : (x / min) * min).ToList();
                return ChangePatternDurations(pattern, newDurations);
            }
            return pattern;
        }

        // Divide by the maximum common divisor, so for ex 4,2,2 is converted to 2,1,1
        private static Pattern Simpl3(Pattern pattern)
        {
            if (pattern.PatternTypeId == PatternType.Pitch) return pattern;

            List<int> RelativeDurations = GetDurationsOfPattern(pattern);

            int gcd = GCD(RelativeDurations.ToArray());
            var newDurations = RelativeDurations.Select(x => x / gcd).ToList();
            return ChangePatternDurations(pattern, newDurations);

        }

        // If the pattern itself has a pattern, get the shortest pattern
        // For ex instead of 2,1,2,1,2,1 we want just 2,1
        private static Pattern Simpl4(Pattern pattern)
        {
            if (pattern.PatternTypeId != PatternType.Rythm) return pattern;

            List<int> RelativeDurations = GetDurationsOfPattern(pattern);
            var newDurations = GetShortestPattern(RelativeDurations);
            return new Pattern
            {
                AsString = string.Join(",", newDurations),
                PatternTypeId = PatternType.Rythm
            };
        }
        private static List<int> GetDurationsOfPattern(Pattern pattern)
        {
            if (pattern.PatternTypeId == PatternType.Pitch) return null;

            if (pattern.PatternTypeId == PatternType.Rythm)
                return (new RythmPattern(pattern)).RelativeDurations;
            else
                return (new MelodyPattern(pattern)).RelativeDurations;
        }
        private static Pattern ChangePatternDurations(Pattern pattern, List<int> durations)
        {
            if (pattern.PatternTypeId == PatternType.Pitch) return null;
            if (pattern.PatternTypeId == PatternType.Rythm)
                return new Pattern
                {
                    AsString = string.Join(",", durations),
                    PatternTypeId = PatternType.Rythm
                };
            else
            {
                var melodyPattern = new MelodyPattern(pattern);
                var newMelodyPattern = new MelodyPattern(melodyPattern.DeltaPitches, durations);
                return new Pattern
                {
                    AsString = newMelodyPattern.AsString,
                    PatternTypeId = PatternType.Melody
                };
            }
        }

        public static List<int> GetShortestPattern(List<int> numbers)
        {
            var divisors = GetDivisorsOfNumber(numbers.Count).OrderByDescending(x => x);
            foreach (int j in divisors)
            {
                int lengthOfGroup = (int)(numbers.Count / j);
                int i = 0;
                int n = 1;
                while (i + n * lengthOfGroup < numbers.Count)
                {
                    if (numbers[i] != numbers[i + n * lengthOfGroup]) break;
                    i++;
                    if (i == lengthOfGroup)
                    {
                        i = 0;
                        n++;
                    }
                }
                if (i + n * lengthOfGroup == numbers.Count)
                {
                    return numbers.Take(lengthOfGroup).ToList();
                }
            }
            return numbers;
        }
        private static IEnumerable<int> GetDivisorsOfNumber(int number)
        {
            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0) yield return i;
            }
        }
        private static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        private static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static Pattern SimplifyPattern(Pattern pattern)
        {

            return Simpl4(Simpl3(Simpl2(Simpl1(pattern))));

        }
    }
}
