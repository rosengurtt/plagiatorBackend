using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {

        /// <summary>
        /// It is possible to have events belonging to different channels in the same chunk
        /// As part of normalization, we create chunks that have only one channel each
        /// 
        /// It is also possible that there are events for one particular channel in 2 different
        /// chunkds.
        /// 
        /// The output of these methods clears this possible mess, so after running this method
        /// we can be sure that there is a 1 to 1 relation between channels and chunks
        /// 
        /// The method takes care only of channel events, all channel independent events are
        /// filtered out
        /// </summary>
        /// <param name="chunks"></param>
        /// <returns></returns>
        private static List<TrackChunk> SeparateChannelsIntoDifferentChunks(ChunksCollection chunks)
        {
            var channelsPresent = new List<int>();
            var allEvents = new List<MidiEvent>();
            foreach (var chunk in chunks)
            {
                long ticksSinceStart = 0;
                var chunko = chunk as TrackChunk;
                if (chunko != null)
                {
                    foreach (var eventito in chunko.Events)
                    {
                        ticksSinceStart += eventito.DeltaTime;
                        var channelDependentEvent = eventito as ChannelEvent;
                        if (channelDependentEvent != null)
                        {
                            var channel = channelDependentEvent.Channel;
                            if (!channelsPresent.Contains(channel))
                                channelsPresent.Add(channel);
                            channelDependentEvent.DeltaTime = ticksSinceStart;
                            allEvents.Add(channelDependentEvent);
                        }
                    }
                }
            }
            var tracks = new List<MidiEvent>[16];
            var retObj = new List<TrackChunk>();
            foreach (MidiEvent eventito in allEvents)
            {
                var e = eventito as ChannelEvent;
                if (e != null)
                {
                    if (tracks[e.Channel.valor] == null)
                        tracks[e.Channel.valor] = new List<MidiEvent>();
                    tracks[e.Channel.valor].Add(e);
                }
            }
            foreach (var track in tracks)
            {
                if (track == null) continue;
                var chunkito = new TrackChunk();
                foreach (var e in ConvertAccumulatedTimeToDeltaTime(track))
                    chunkito.Events.Add(e);
                retObj.Add(chunkito);
            }
            return retObj;
        }

    }
}
