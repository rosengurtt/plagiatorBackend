using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// Used for a join table that associates notes to melodies
    /// </summary>
    public class MelodyNote
    {
        public long Id { get; set; }
        public long MelodyId { get; set; }
        public long NoteId { get; set; }
    }
}
