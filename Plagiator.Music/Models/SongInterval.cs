using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// Represents a time space inside a song, defined by the start and end in ticks
    /// </summary>
    public class SongInterval
    {
        public long StartInTicksSinceBeginningOfSong { get; set; }
        public long EndInTicksSinceBeginningOfSong { get; set; }

        public string AsString()
        {
            return $"{StartInTicksSinceBeginningOfSong}-{EndInTicksSinceBeginningOfSong}";
        }
        public static SongInterval FromString(string s)
        {
            var parts = s.Split("-");
            return new SongInterval()
            {
                StartInTicksSinceBeginningOfSong = int.Parse(parts[0]),
                EndInTicksSinceBeginningOfSong = int.Parse(parts[1])
            };
        }
    }
}
