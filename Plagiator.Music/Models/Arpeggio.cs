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
    public class Arpeggio
    {
        public int Id { get; set; }

        List<ArpeggioOccurrence> ArpeggioOccurrences { get; set; }

        /// <summary>
        /// Represents the intervals in the arpeggio in semitones
        /// For ex. C G C E C G C E would be
        /// 0 7 0 4
        /// </summary>
        [NotMapped]
        public List<int> PitchPattern { get; set; }

        public string PitchPatternString
        {
            get
            {
                return String.Join(",", PitchPattern);
            }
            set
            {
                PitchPattern = Array.ConvertAll(value.Split(","), s => int.Parse(s)).ToList();
            }
        }

        /// <summary>
        /// Represents the relative lenghts between the start of the notes
        /// If all lengths are equal, then RithmPattern = [1]
        /// If the lengths are quarter, eight, eight, quarter, quarter,
        /// then RythmPattern is [2,1,2,2]
        /// </summary>
        [NotMapped]
        public List<int> RythmPattern { get; set; }

        public string RythmPatternString
        {
            get
            {
                return String.Join(",", RythmPattern);
            }
            set
            {
                RythmPattern = Array.ConvertAll(value.Split(","), s => int.Parse(s)).ToList();
            }
        }


        public List<int> intervals
        {
            get
            {
                var retObj = new List<int>();
                for (int i = 0; i < PitchPattern.Count - 1; i++)
                {
                    for (int j = i + 1; j < PitchPattern.Count; j++)
                    {
                        if (!retObj.Contains((j - i + 48) % 12))
                            retObj.Add((j - i + 48) % 12);
                    }
                }
                return retObj;
            }
        }

        /// <summary>
        /// I don't want to bother implementing GetHashCode, so rather than overriding
        /// Equals, I create this method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Arpeggio arpi = (Arpeggio)obj;
                for(int i= 0; i < this.PitchPattern.Count;i++)
                {
                    if (this.PitchPattern[i] != arpi.PitchPattern[i]) return false;
                }
                for (int i = 0; i < this.RythmPattern.Count; i++)
                {
                    if (this.RythmPattern[i] != arpi.RythmPattern[i]) return false;
                }
                return true;
            }
        }
    }
}
