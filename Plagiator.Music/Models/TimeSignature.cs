using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents the standard time signature used in music, a couple of numbers expressed as a fraction
    /// </summary>
    public class TimeSignature
    {
        public int Id { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
    }
}
