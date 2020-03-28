using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// We create simplificated versions of songs by removing less important notes
    /// We remove gradually more and more notes and create more simplified versions
    /// </summary>
    public class SongSimplification
    {
        public long Id { get; set; }
        public long SongId { get; set; }

        /// <summary>
        /// This number increases with each simplification we make of a song
        /// Version 0 is the original song with all the notes
        /// </summary>
        public long SimplificationVersion { get; set; }
        public List<Note> Notes { get; set; }

        public List<Occurrence> Occurrences { get; set; }


        public List<Note> NotesOfInstrument(int instr)
        {
            var retObj = new List<Note>();
            foreach (var n in Notes)
            {
                if (n.Instrument == instr)
                    retObj.Add(n.Clone());
            }
            return retObj;
        }
    }
}

