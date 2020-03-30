using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Plagiator.Models.Entities
{

    /// <summary>
    /// The main reason for creating this class is for persistence in sql
    /// The useful classes are PitchPattern, RythmPattern and MelodyPattern, but to avoid creating
    /// 3 different tables for each, we define this entity that is kind of a parent class of the
    /// PitchPattern, RythmPattern and MelodyPattern classes
    /// </summary>
    public class Pattern
    {
        public long Id { get; set; }

        /// <summary>
        /// We save patterns as a string in the database.
        /// The elements of the patterns are separated by commas. 
        /// 
        /// A pitch pattern looks like: 
        /// 0,-2,-1,-2
        /// Each digit is a pitch step (the pitch difference with the previous note)
        /// 
        /// A rythm pattern looks like:
        ///	1,2,4,2,4,2
        ///	Each digit is a duration
        ///	
        /// A Melody pattern looks like:
        /// (0-1),(-2-1),(-1-1),(-2-1)
        /// In this case, we have pairs of digits, the first being a pitch step and the second a duration
        /// </summary>
        public string AsString { get; set; }

        [NotMapped]
        public int Length
        {
            get
            {
                return AsString.Split(",").Length;
            }
        }

        public PatternType PatternTypeId { get; set; }

        public Pattern Clone()
        {
            return new Pattern()
            {
                AsString = this.AsString,
                PatternTypeId = this.PatternTypeId
            };
        }

    }
}
