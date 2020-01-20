using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BandId { get; set; }
        public Band Band { get; set; }
        public int StyleId { get; set; }
        public Style Style { get; set; }
        public int? Tempo { get; set; }
        public int? NumberTracks { get; set; }
        public int? NumberBars { get; set; }
        public bool? HasPercusion { get; set; }
        public bool? HasChords { get; set; }
        public bool? HasMelody { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public string OriginalMidiBase64Encoded { get; set; }
        public string NormalizedMidiBase64Encoded { get; set; }

    }
}
