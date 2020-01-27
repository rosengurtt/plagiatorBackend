using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<ChannelEvent> GetEventsOfChannel(MidiFile midiFile, int channel)
        {
            var retObj = new List<ChannelEvent>();
            midiFile = ConvertDeltaTimeToAccumulatedTime(midiFile);
            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                foreach (var eventito in chunk.Events)
                {
                    if (eventito is ChannelEvent && ((ChannelEvent)eventito).Channel == channel)
                        retObj.Add((ChannelEvent)eventito);
                }
            }
            return retObj;
        }
        public static List<ChannelEvent> GetEventsOfChannel(string base64encodedMidiFile, int channel)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            return GetEventsOfChannel(midiFile, channel);
        } 
    }
}
