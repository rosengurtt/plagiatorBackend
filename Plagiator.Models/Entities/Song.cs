using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    public class Song
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MidiBase64Encoded { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public long BandId { get; set; }
        public Band Band { get; set; }
        public long StyleId { get; set; }
        public Style Style { get; set; }
        public long SongStatsId { get; set; }
        public SongStats SongStats { get; set; }
        public List<SongSimplification> SongSimplifications { get; set; }
        public List<Bar> Bars { get; set; }

        public List<TempoChange> TempoChanges { get; set; }
    }
}
