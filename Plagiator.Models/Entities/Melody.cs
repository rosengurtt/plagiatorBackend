using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// A group of notes played consecutevily with the same instrument
    /// </summary>
    public class Melody
    {
        public List<Note> Notes { get; set; }
        public byte Instrument { get; set; }

    }
}
