using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents the intervals in the arpeggio in semitones
    /// For ex. C G C E C G C E would be
    /// 0 7 0 4
    /// </summary>
    public class PitchPattern
    {
        public int Id { get; set; }

        private List<int> pitchesRelativeToFirst { get; set; }

        [NotMapped]
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

        public List<Arpeggio> Arpeggios { get; set; }

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
