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


        public int PitchPatternId { get; set; }
        public PitchPattern PitchPattern { get; set; }

        public int RythmPatternId { get; set; }
        public RythmPattern RythmPattern { get; set; }


        /// <summary>
        /// I don't want to bother implementing GetHashCode, so rather than overriding
        /// Equals, I create this method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Arpeggio arpi = (Arpeggio)obj;
                if (PitchPattern.AsString != arpi.PitchPattern.AsString)
                    return false;
                if (RythmPattern.AsString != arpi.RythmPattern.AsString)
                    return false;
                return true;
            }
        }
    }
}
