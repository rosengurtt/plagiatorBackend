using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        /// <summary>
        /// Finds sequences of numbers that appear more than once in the list of numbers
        /// The dictionary that returns has
        /// - in the keys the sequences found
        /// - in the values the locations in the list of numbers where the sequences starts
        /// 
        /// For example if the sequence 1,4,1,5 was found in the index 35 and 76 of of the list 'numbers'
        /// the dictionary will have an entry with key=[1,4,1,5] and value [35,76]
        /// </summary>
        /// <param name="numbers"></param>
        /// <param name="minLength">The minimum length of the sequences to find</param>
        /// <param name="maxLength">The maximum length of the sequences to find</param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> FindPatternsInListOfIntegers(List<int> numbers, int minLength, int maxLength)
        {
            // This dictionary stores all the sequences of up to 50 numbers
            var sequences = new Dictionary<string, List<int>>();

            for (int i=0; i < numbers.Count - minLength; i++)
            {
                for(int j=minLength; j <= maxLength; j++)
                {
                    if (i + j  >= numbers.Count) break;
                    var seq = ListOfNumbersAsString(numbers.GetRange(i,j));
                    if (!sequences.Keys.Contains(seq))
                        sequences[seq] = new List<int>();
                    sequences[seq].Add(i);
                }
            }
            sequences = RemoveSingletons(sequences);
            return RemoveSubsequences(sequences);
        }

        /// <summary>
        /// If we find the pattern 1,2,3,4 we will also find the patterns 1,2,3 and 2,3,4
        /// But if 1,2,3 and 2,3,4 never happer on their own, but always as part of 1,2,3,4
        /// then we remove them
        /// </summary>
        /// <param name="sequences"></param>
        /// <returns></returns>
        private static Dictionary<string, List<int>> RemoveSubsequences(Dictionary<string, List<int>> sequences)
        {
            var retObj = new Dictionary<string, List<int>>();
            var sortedKeys = sequences.Keys.OrderBy(x => x).ToList();
            for (int i = 0; i < sequences.Keys.Count - 1; i++)
            {
                if (!sortedKeys[i + 1].Contains(sortedKeys[i]) ||
                    sequences[sortedKeys[i]].Count > sequences[sortedKeys[i + 1]].Count)
                    retObj[sortedKeys[i]] = sequences[sortedKeys[i]];
            }
            return retObj;
        }

        private static Dictionary<string, List<int>> RemoveSingletons(Dictionary<string, List<int>> sequences)
        {
            var retObj = new Dictionary<string, List<int>>();
            foreach (string key in sequences.Keys)
                if (sequences[key].Count > 1)
                    retObj[key]=sequences[key];
            return retObj;
        }
        private static string ListOfNumbersAsString(List<int> numbers)
        {
            return String.Join(",", numbers);
        }

    }
}
