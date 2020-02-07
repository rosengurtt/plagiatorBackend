using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    public class TempoChange
    {
        public int Id { get; set; }

        public int SongId { get; set; }

        public int MicrosecondsPerQuarterNote { get; set; }

        public long TicksSinceBeginningOfSong { get; set; }

        public int TempoAsBeatsPerQuarterNote
        {
            get
            {
                return 120 * 500000 / MicrosecondsPerQuarterNote;
            }
        }
    }
}
