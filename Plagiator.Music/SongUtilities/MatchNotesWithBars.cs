using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    /// <summary>
    /// In a given normalized song, fills the notes field of bars, and the
    /// bar field of notes. In othe word, links each bar to the notes in the
    /// bar and each note with its corresponding bar
    /// </summary>
    public partial class MidiProcessing
    {
        public static NormalizedSong MatchNotesWithBars(NormalizedSong song)
        {
            // Initialize bars as empty
            foreach (var bar in song.Bars)
                bar.Notes = new List<Note>();
            foreach (var n in song.Notes)
            {
                AddNoteToBars(song.Bars, n, song.TicksPerBeat);
            }
            return song;
        }

        private static void AddNoteToBars(List<Bar> bars, Note n, int ticksPerBeat)
        {
            n.Bars = new List<Bar>();
            foreach (var bar in bars)
            {
                int barLengthInTicks = bar.TimeSignature.Numerator * ticksPerBeat;
                var barStart = bar.TicksFromBeginningOfSong;
                var noteStart = n.StartSinceBeginningOSongInTicks;
                var noteEnd = n.EndSinceBeginnintOfSongInTicks;
                var barEnd = bar.TicksFromBeginningOfSong + barLengthInTicks;
                if (barEnd >= noteStart || noteEnd >= barStart) continue;
                if (!n.Bars.Contains(bar))
                    n.Bars.Add(bar);
                if (!bar.Notes.Contains(n))
                    bar.Notes.Add(n);
            }
        }
    }
}


