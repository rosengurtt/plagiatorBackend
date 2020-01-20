using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music
{
    /// <summary>
    /// Represents the standard time signature used in music, a couple of numbers expressed as a fraction
    /// </summary>
    public class TimeSignature
    {
        public int numerator { get; set; }
        public int denominator { get; set; }
    }
}
