using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Plagiator.Music.Models
{

    public class MelodyPattern
    {
        public int Id { get; set; }

        List<MelodyPatternOccurrence> MelodyPatternOccurrences { get; set; }


        public int PitchPatternId { get; set; }
        public PitchPattern PitchPattern { get; set; }

        public int RythmPatternId { get; set; }
        public RythmPattern RythmPattern { get; set; }

        [NotMapped]
        public int Length
        {
            get
            {
                return PitchPattern.Length;
            }
        }
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
                MelodyPattern arpi = (MelodyPattern)obj;
                if (PitchPattern.AsString != arpi.PitchPattern.AsString)
                    return false;
                if (RythmPattern.AsString != arpi.RythmPattern.AsString)
                    return false;
                return true;
            }
        }
    }
}
