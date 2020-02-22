using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using System.Collections.Generic;

namespace Plagiator.Music.Models
{
    public class Note
    {
        public Note()
        {
            PitchBending = new List<PitchBendItem>();
        }
        public long Id { get; set; }

        public int SongVersionId { get; set; }
        public SongVersion SongVersion { get; set; }
        public byte Pitch { get; set; }
        public byte Volume { get; set; }
        public long StartSinceBeginningOfSongInTicks { get; set; }
        public long EndSinceBeginningOfSongInTicks { get; set; }
        public bool IsPercussion { get; set; }


        public GeneralMidi2Program Instrument { get; set; }
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
