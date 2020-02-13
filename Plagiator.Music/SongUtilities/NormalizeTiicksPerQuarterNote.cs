

using Melanchall.DryWetMidi.Core;
using System;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static string NormalizeTicksPerQuarterNote(string base64encodedMidiFile)
        {
            int standardTicksPerQuarterNote = 96;
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            midiFile = ConvertDeltaTimeToAccumulatedTime(midiFile);
            var originalTicksPerQuarterNote=MidiProcessing.GetTicksPerBeatOfSong(base64encodedMidiFile);

            double conversionRate = standardTicksPerQuarterNote / (double)originalTicksPerQuarterNote;
            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                foreach(var eventito in chunk.Events)
                {
                    eventito.DeltaTime = (long)Math.Round(conversionRate * eventito.DeltaTime);
                }
            }
            midiFile = ConvertAccumulatedTimeToDeltaTime(midiFile);
            return MidiProcessing.ConvertMidiFileToBase64encodedMidiFile(midiFile);
        }
    }
}
