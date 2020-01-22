using System;
using System.Collections.Generic;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Mucic.Utilities;

namespace Plagiator.Music
{
     public class NormalizedSong
    {
        public NormalizedSong(string base64encodedMidi)
        {
            TicksPerBeat = MidiProcessing.GetTicksPerBeatOfSong(base64encodedMidi);
            ChannelIndependentEvents = MidiProcessing.ExtractChannelIndependentEvents(base64encodedMidi);
            Bars = MidiProcessing.GetEmptyBarsOfSong(base64encodedMidi);
            Notes = MidiProcessing.GetNotesOfSong(base64encodedMidi);
            Instruments = MidiProcessing.GetInstruments(Notes);
            Tracks = MidiProcessing.GetNormalizedNotesChuncks(this);
        }
        public int TicksPerBeat { get; set; }
        public List<GeneralMidi2Program> Instruments { get; set; }
        public List<Bar> Bars { get; set; }
        public List<Note> Notes { get; set; } 
        public List<TrackChunk> Tracks { get; set; }

        public List<MidiEvent> ChannelIndependentEvents { get; set; }


        public static string GetSongAsBase64EncodedMidi(string base64encodedMidi)
        {
            var normalita = new NormalizedSong(base64encodedMidi);
            return base64encodedMidi;
        }
    }
}
