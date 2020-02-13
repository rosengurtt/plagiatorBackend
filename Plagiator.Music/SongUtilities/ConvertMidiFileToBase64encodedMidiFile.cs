using Melanchall.DryWetMidi.Core;
using System;
using System.IO;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static string ConvertMidiFileToBase64encodedMidiFile(MidiFile midiFile)
        {
            using (MemoryStream memStream = new MemoryStream(1000000))
            {
                midiFile.Write(memStream);
                var bytes = memStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
