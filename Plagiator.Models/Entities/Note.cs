﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Models.Entities
{
    public class Note
    {
        public Note()
        {
            PitchBending = new List<PitchBendItem>();
        }
        public long Id { get; set; }

        public long SongSimplificationId { get; set; }
        public SongSimplification SongSimplification { get; set; }
        public byte Pitch { get; set; }
        public byte Volume { get; set; }
        public long StartSinceBeginningOfSongInTicks { get; set; }
        public long EndSinceBeginningOfSongInTicks { get; set; }
        public bool IsPercussion { get; set; }

        /// <summary>
        /// Basically, the same as a track
        /// If there were no cases of 2 tracks with the same intrument, we would not need it
        /// But because we may have 2 pianos for example, if we don't keep separated we
        /// loose important information
        /// </summary>
        public byte Voice { get; set; }


        public byte Instrument { get; set; }
        public List<PitchBendItem> PitchBending { get; set; }

        public int DurationInTicks
        {
            get
            {
                return (int)(EndSinceBeginningOfSongInTicks - StartSinceBeginningOfSongInTicks);
            }
        }

        public Note Clone()
        {
            var bendItems = new List<PitchBendItem>();
            foreach (var b in PitchBending)
            {
                bendItems.Add(b.Clone());
            }
            return new Note
            {
                EndSinceBeginningOfSongInTicks = this.EndSinceBeginningOfSongInTicks,
                StartSinceBeginningOfSongInTicks = this.StartSinceBeginningOfSongInTicks,
                Pitch = this.Pitch,
                Volume = this.Volume,
                Instrument = this.Instrument,
                PitchBending = bendItems,
                IsPercussion = this.IsPercussion
            };
        }
        public bool IsEqual(object n)
        {
            //Check for null and compare run-time types.
            if ((n == null) || !this.GetType().Equals(n.GetType()))
            {
                return false;
            }
            else
            {
                Note noty = (Note)n;
                if (noty.Pitch == Pitch &&
                    noty.StartSinceBeginningOfSongInTicks == StartSinceBeginningOfSongInTicks &&
                    noty.EndSinceBeginningOfSongInTicks == EndSinceBeginningOfSongInTicks &&
                    noty.Instrument == Instrument)
                    return true;
                return false;
            }
        }
    }
}

