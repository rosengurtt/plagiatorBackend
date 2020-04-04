using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis.Patterns
{
    public  static partial class PatternUtilities
    {
        /// <summary>
        /// Finds sequences of strings that appear more than once in the list of string
        /// The strings must not have a comma, because the comma is used as a separator when
        /// writing a sequence of strings as one string
        /// 
        /// We can use this method for looking patterns in lists of notes, if we express the notes
        /// as a couple of numbers separated by some separator that is not a comma. We use this notation
        /// for notes: 
        /// 
        /// (3-5)
        /// 
        /// where the first number (3) is the pitch and the second (5) the duration.
        /// 
        /// 
        /// The dictionary that returns has
        /// - in the keys the sequences found
        /// - in the values the locations in the list of numbers where the sequences starts
        /// 
        /// For example if the sequence ["1","4","1","5"] was found in the index 35 and 76 of of the list 'strings'
        /// the dictionary will have an entry with key=["1","4","1","5"] and value [35,76]
        /// </summary>
        /// <param name="numbers"></param>
        /// <param name="minLength">The minimum length of the sequences to find</param>
        /// <param name="maxLength">The maximum length of the sequences to find</param>
        /// <returns></returns>
        public static Dictionary<string, List<int>> FindPatternsInListOfStrings(List<string> strings, int minLength, int maxLength)
        {
            // This dictionary stores all the sequences of up to 50 numbers
            var sequences = new Dictionary<string, List<int>>();

            for (int i = 0; i < strings.Count - minLength; i++)
            {
                for (int j = minLength; j <= maxLength; j++)
                {
                    if (i + j >= strings.Count) break;
                    var seq = ListOfNumbersAsString(strings.GetRange(i, j));
                    if (!sequences.Keys.Contains(seq))
                        sequences[seq] = new List<int>();
                    sequences[seq].Add(i);
                }
            }
            sequences = RemoveSingletons(sequences);
            return RemoveSubsequences(sequences);
        }

        public static Dictionary<string, List<int>> FindPatternsInListOfIntegers(List<int> integers, int minLength, int maxLength)
        {
            var listOfStrings = new List<string>();
            foreach (var x in integers) listOfStrings.Add(x.ToString());
            return FindPatternsInListOfStrings(listOfStrings, minLength, maxLength);
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
                    retObj[key] = sequences[key];
            return retObj;
        }
        private static string ListOfNumbersAsString(List<string> numbers)
        {
            return String.Join(",", numbers);
        }
    }
}
