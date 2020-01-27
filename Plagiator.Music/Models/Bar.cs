using System;
using System.Collections.Generic;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents a bar in a song (in Spanish "un compas")
    /// </summary>
    public class Bar
    {
        public long TicksFromBeginningOfSong { get; set; }
        
        public TimeSignature TimeSignature { get; set; }

        /// <summary>
        /// Microseconds per quarter note
        /// </summary>
        public long Tempo { get; set; }

        public List<Note> Notes { get; set; }

    }
}
