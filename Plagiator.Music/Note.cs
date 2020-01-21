using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using System.Collections.Generic;

namespace Plagiator.Music
{

    public class Note
    {
        public int Pitch { get; set; }
        public long StartSinceBeginningOSongInTicks { get; set; }
    
        public long EndSinceBeginnintOfSongInTicks { get; set; }

        public GeneralMidi2Program Instrument { get; set; }

        public List<PitchBendEvent> PitchBendingEvents { get; set; }
        public List<ControlChangeEvent> ControlChangeEvents { get; set; }
    }
}
