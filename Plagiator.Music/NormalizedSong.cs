using System;
using System.Collections.Generic;
using System.Text;
using Plagiator.Mucic.Utilities;

namespace Plagiator.Music
{
     public class NormalizedSong
    {
        public NormalizedSong(string base64encodedMidi)
        {
            TicksPerBeat = MidiProcessing.GetTicksPerBeatOfSong(base64encodedMidi);
            Bars = MidiProcessing.GetAllMusicalEventsOfSong(base64encodedMidi);
        }
        public int TicksPerBeat { get; set; }
        public List<Bar> Bars { get; set; }
        public List<Note> Notes { get; set; }

        public string GetSongAsBase64EncodedMidi(string base64encodedMidi)
        {
            Bars = MidiProcessing.GetEmptyBarsOfSong(base64encodedMidi);
            Notes = MidiProcessing.GetNotesOfSong(base64encodedMidi);
            return base64encodedMidi;
        }
    }
}
