using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Plagiator.Models.Entities
{

    /// <summary>
    /// Represents an association of a pattern with a place in a song (more precisely the simplification of a song)
    /// where the pattern is used
    /// The association is done through the note in the song where the pattern starts
    /// </summary>
    public class Occurrence
    {
        public long Id { get; set; }

        public long PatternId { get; set; }
        public Pattern Pattern { get; set; }

        [NotMapped]
        public List<Note> Notes { get; set; }

        public long SongSimplificationId { get; set; }
        public SongSimplification SongSimplification { get; set; }

        public Occurrence Clone()
        {
            var retObj = new Occurrence();
            retObj.Pattern = new Pattern
            {
                AsString = this.Pattern.AsString,
                PatternTypeId = this.Pattern.PatternTypeId
            };
            retObj.SongSimplification = this.SongSimplification;
            retObj.Notes = this.Notes;
            return retObj;
        }
    }

}

