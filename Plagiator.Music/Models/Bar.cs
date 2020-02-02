
using System;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents a bar in a song (in Spanish "un compas")
    /// </summary>
    public class Bar
    {
        public Bar(
            int barNumber,
            long ticksFromBeginningOfSong,
            TimeSignature timeSignature,
            long tempoInMicrosecondsPerQuarterNote)
        {
            BarNumber = barNumber;
            TicksFromBeginningOfSong = ticksFromBeginningOfSong;
            TimeSignature = timeSignature;
            Tempo = new Tempo {
                MicrosecondsPerQuarterNote = tempoInMicrosecondsPerQuarterNote
            };
        }
        public int BarNumber { get; }
        public long TicksFromBeginningOfSong { get;  }
        
        public TimeSignature TimeSignature { get;  }

        /// <summary>
        /// This flag is used when quantizing the duration of notes
        /// We aproximate the durations to whole quarters, quavers, etc.
        /// and we don't want to aproximate a triplet duration by a quaver
        /// </summary>
        public bool HasTriplets { get; set; }

   
        public Tempo Tempo { get; }


    }
}
