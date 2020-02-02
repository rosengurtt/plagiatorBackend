using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.Models;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {

        private static GeneralMidi2Program GetInstrument(List<MidiEvent> events, int index)
        {
            var eventito = events[index];
            var instrumentCode = ((ProgramChangeEvent)(eventito)).ProgramNumber;
            return (GeneralMidi2Program)(instrumentCode.valor);
        }

        public static List<GeneralMidi2Program> GetInstruments(List<Note> notes)
        {
            var instruments = new List<GeneralMidi2Program>();
            foreach (var n in notes)
            {
                if (!instruments.Contains(n.Instrument))
                {
                    instruments.Add(n.Instrument);
                }
            }
            return instruments.OrderBy(x => (int)x).ToList();
        }

    }
}
