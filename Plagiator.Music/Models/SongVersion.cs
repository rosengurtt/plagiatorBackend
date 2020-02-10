using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    public class SongVersion
    {
        public int Id { get; set; }
        public int SongId { get; set; }

        public int VersionNumber { get; set; }
        public List<Note> Notes { get; set; }
    }
}
