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

        public byte Pitch { get; set; }
        public byte Volume { get; set; }
        public long StartInTicks { get; set; }
    
        public long EndInTicks { get; set; }

        public GeneralMidi2Program Instrument { get; set; }
        public List<PitchBendItem> PitchBending { get; set; }

        public int DurationInTicks
        {
            get
            {
                return (int)(EndInTicks -
                    StartInTicks);
            }
        }
        
        public Note Clone()
        {
            var bendItems = new List<PitchBendItem>();
            foreach( var b in PitchBending)
            {
                bendItems.Add(b.Clone());
            }
            return new Note
            {
                EndInTicks = this.EndInTicks,
                StartInTicks = this.StartInTicks,
                Pitch = this.Pitch,
                Volume = this.Volume,
                Instrument=this.Instrument,
                PitchBending = bendItems
            };
        }


    }
}
