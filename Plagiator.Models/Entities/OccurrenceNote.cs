using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    public class OccurrenceNote
    {
        public long Id { get; set; }
        public long OccurrenceId { get; set; }
        public long NoteId { get; set; }
    }
}
