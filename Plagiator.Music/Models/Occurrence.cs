using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents an association of a pattern with a place in a song (more precisely the version of a song)
    /// where the pattern is used
    /// The association is done through the note in the song where the pattern starts
    /// </summary>
    public class Occurrence
    {
        public long Id { get; set; }

        public long PatternId { get; set; }
        public Pattern Pattern { get; set; }
        
        public long FirstNoteId { get; set; }
        public Note FirstNote { get; set; }
        public long LastNoteId { get; set; }
        public Note LastNote { get; set; }

        public long SongVersionId { get; set; }
        public SongVersion SongVersion { get; set; }

    }

}

