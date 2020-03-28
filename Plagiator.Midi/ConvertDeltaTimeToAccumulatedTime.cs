﻿using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Midi
{
    public static partial class MidiUtilities
    {
        public static List<MidiEvent> ConvertDeltaTimeToAccumulatedTime(List<MidiEvent> list)
        {
            var returnObj = new List<MidiEvent>();
            long accumulatedTime = 0;
            foreach (var e in list)
            {
                var clonito = e.Clone();
                accumulatedTime += e.DeltaTime;
                clonito.DeltaTime = accumulatedTime;
                returnObj.Add(clonito);
            }
            return returnObj;
        }
        public static MidiFile ConvertDeltaTimeToAccumulatedTime(MidiFile midiFile)
        {
            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                chunk.Events._events = ConvertDeltaTimeToAccumulatedTime(chunk.Events._events);
            }
            return midiFile;
        }
    }
}
