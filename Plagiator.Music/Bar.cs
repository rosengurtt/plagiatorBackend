using System;

namespace Plagiator.Music
{
    /// <summary>
    /// Represents a bar in a song (in Spanish "un compas")
    /// </summary>
    public class Bar
    {
        public int TicksFromBeginningOfSong { get; set; }
        
        public TimeSignature TimeSignature { get; set; }

        /// <summary>
        /// Microseconds per quarter note
        /// </summary>
        public long Tempo { get; set; }


    }
}
