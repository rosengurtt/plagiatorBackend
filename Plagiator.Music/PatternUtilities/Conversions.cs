using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        public static List<int> ConvertToListOfIntegers(List<string> strings)
        {
            var retObj = new List<int>();
            foreach (var s in strings)
                retObj.Add(int.Parse(s));
            return retObj;
        }

        public static List<string> ConvertToListOfStrings(List<int> integers)
        {
            var retObj = new List<string>();
            foreach (var i in integers)
                retObj.Add(i.ToString());
            return retObj;
        }
    }
}
