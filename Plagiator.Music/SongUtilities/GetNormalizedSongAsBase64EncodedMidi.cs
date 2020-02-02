using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing 
    {
        //public string GetNormalizedSongAsBase64EncodedMidi(NormalizedSong song)
        //{

        //}

        /// <summary>
        /// If there was an unlimited number of channels available, or if
        /// the number of instruments in the song was always less or equal
        /// than the channels, we would assign each instrument a different
        /// channel. But because the max number of channels is 16 (1 dedicated
        /// to percussion), then we have to share a channel between instruments
        /// that don't play at the same time.
        /// This method assigns each instrument a channel, and when needed#
        /// assigning 2 or more instruments that don't overlap the same channel
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        //private Dictionary<int,int> GetChannelsForInstruments(NormalizedSong song)
        //{

        //}
    }
}
