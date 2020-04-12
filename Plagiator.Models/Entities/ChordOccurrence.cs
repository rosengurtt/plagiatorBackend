using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    public class ChordOccurrence
    {
        public long Id { get; set; }

        public long ChordId { get; set; }
        public Chord Chord { get; set; }
        public long SongSimplificationId { get; set; }
        public SongSimplification SongSimplification { get; set; }
        public long StartTick { get; set; }
        public long EndTick { get; set; }
    }
}
