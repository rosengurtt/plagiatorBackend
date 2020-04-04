﻿using Melanchall.DryWetMidi.Core;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Midi
{
    public static partial class MidiUtilities
    {
        public static string GetInstrumentsAsString(string base64encodedMidiFile)
        {
            var instruments = new List<int>();
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            var programChangeEvents = GetEventsOfType(midiFile, MidiEventType.ProgramChange);
            foreach (var eve in programChangeEvents)
            {
                var ev = eve as ProgramChangeEvent;
                if (!instruments.Contains(ev.ProgramNumber)) instruments.Add(ev.ProgramNumber);
            }
            return string.Join((","), instruments);
        }
    }
}
