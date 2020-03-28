using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    public class TempoChange
    {
        public long Id { get; set; }

        public long SongId { get; set; }

        public long MicrosecondsPerQuarterNote { get; set; }

        public long TicksSinceBeginningOfSong { get; set; }

        public long TempoAsBeatsPerQuarterNote
        {
            get
            {
                return 120 * 500000 / MicrosecondsPerQuarterNote;
            }
        }
    }
}