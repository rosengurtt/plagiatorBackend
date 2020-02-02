using System;

namespace Plagiator.Music.Models
{
    public class Tempo
    {
        public long MicrosecondsPerQuarterNote { get; set; }
        public int BeatsPerMinute
        {
            get
            {
                return (int)Math.Floor(120 * ((double)500000 / (double)MicrosecondsPerQuarterNote));
            }
        }
    }
}
