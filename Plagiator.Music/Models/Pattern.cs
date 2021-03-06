﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// The main reason for creating this class is for persistence in sql
    /// The useful classes are PitchPattern, RythmPattern and MelodyPattern
    /// </summary>
    public class Pattern
    {
        public long Id { get; set; }

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
    public enum PatternType : Byte
    {
        Pitch = 1,
        Rythm = 2,
        Melody = 3
    }
}
