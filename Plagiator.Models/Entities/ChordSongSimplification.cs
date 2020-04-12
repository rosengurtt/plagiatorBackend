using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// Needed for the join table of Chords and SongSimplifications
    /// </summary>
    public class ChordSongSimplification
    {
        public long ChordId { get; set; }
        public Chord Chord { get; set; }
        public long SongSimplificationId { get; set; }
        public SongSimplification SongSimplification { get; set; }
    }
}
