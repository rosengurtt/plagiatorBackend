using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// Represents a group of notes played at the same time
    /// It consists of notes that belong to a SongSimplification
    /// The chord is associated to a song simplification
    /// </summary>
    public class Chord
    {
        public Chord() { }
        public Chord(List<Note> notes)
        {
            var latestStart = notes
                .Select(n => n.StartSinceBeginningOfSongInTicks).Max();
            var earlierEnd = notes
                .Select(n => n.EndSinceBeginningOfSongInTicks).Min();
            if (latestStart > earlierEnd)
                throw new Exception("Notes not playing at the same time");
            Notes = notes;
        }

        public long Id { get; set; }
        public List<Note> Notes { get; set; }

        public long SongSimplificationId { get; set; }

        public List<int> Pitches
        {
            get
            {
                var retObj = new List<int>();
                foreach(var n in Notes)
                {
                    if (!retObj.Contains(n.Pitch)) retObj.Add(n.Pitch);
                }
                return retObj;
            }
        }
        public long StartTick
        {
            get
            {
                return Notes.Select(n => n.StartSinceBeginningOfSongInTicks).Min();
            }
        }
        public long EndTick
        {
            get
            {
                return Notes.Select(n => n.EndSinceBeginningOfSongInTicks).Max();
            }
        }
    }
}
