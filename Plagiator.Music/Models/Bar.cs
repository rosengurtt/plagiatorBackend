
using System;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents a bar in a song (in Spanish "un compas")
    /// </summary>
    public class Bar
    {

        public long Id { get; set; }
        public int BarNumber { get; set; }
        public long TicksFromBeginningOfSong { get; set; }
        
        public long TimeSignatureId { get; set; }
        public TimeSignature TimeSignature { get; set; }

        /// <summary>
        /// This flag is used when quantizing the duration of notes
        /// We aproximate the durations to whole quarters, quavers, etc.
        /// and we don't want to aproximate a triplet duration by a quaver
        /// </summary>
        public bool HasTriplets { get; set; }

   
        public int TempoInMicrosecondsPerQuarterNote { get; set; }


    }
}
