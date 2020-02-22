using Melanchall.DryWetMidi.Standards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    public class SongVersion
    {
        public long Id { get; set; }
        public long SongId { get; set; }

        public int VersionNumber { get; set; }
        public List<Note> Notes { get; set; }

        public List<Occurrence> Occurrences { get; set; }


        public List<Note> NotesOfInstrument(GeneralMidi2Program instr)
        {
            var retObj = new List<Note>();
            foreach(var n in Notes)
            {
                if (n.Instrument == instr)
                    retObj.Add(n.Clone());
            }
            return retObj;
        }
    }
}
