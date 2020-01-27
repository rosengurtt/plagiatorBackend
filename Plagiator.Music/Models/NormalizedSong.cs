using System;
using System.Collections.Generic;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.SongUtilities;

namespace Plagiator.Music.Models
{
     public class NormalizedSong
    {
        /// <summary>
        /// The idea of this class is to have an object that is easy to manipulate
        /// for the purpose of composing
        /// 
        /// The changes we do to the original midi file are:
        /// 
        /// - For each note played in the song, we have 
        ///     + the complete information of when it was played (time since beginning of song)
        ///     + the volume (extracted from the velocity value), 
        ///     + we take in consideration the sustain pedal by extending the time of the note
        ///       and removing the pedal event
        ///     + the pitch bending events (with the time of the event as the number of ticks since beginning of song)
        /// - We have the tempo events that make significant changes to the tempo (more than 10%),
        ///   again with time reference to the beginning of the song
        /// - We have bars information. So we can go straight to bar 21 and see the notes there
        /// - We have one track for each different instrument
        /// - We have the time signature changes, since they affect the bars
        /// - We discard other effects, as aftertouch, pedals (except sustain), etc.
        /// </summary>
        /// <param name="base64encodedMidi"></param>
        public NormalizedSong(string base64encodedMidi)
        {
            TicksPerBeat = MidiProcessing.GetTicksPerBeatOfSong(base64encodedMidi);
            ChannelIndependentEvents = MidiProcessing.GetChannelIndependentEvents(base64encodedMidi);
            Bars = MidiProcessing.GetEmptyBarsOfSong(base64encodedMidi);
            Notes = MidiProcessing.GetNotesOfSong(base64encodedMidi);
            PercussionNotes = MidiProcessing.GetNotesOfSong(base64encodedMidi, true);
            Instruments = MidiProcessing.GetInstruments(Notes);
            Tracks = MidiProcessing.GetNormalizedNotesChunks(this);
            MidiProcessing.MatchNotesWithBars(this);
        }
        public int TicksPerBeat { get; set; }
        public List<GeneralMidi2Program> Instruments { get; set; }
        public List<Bar> Bars { get; set; }
        public List<Note> Notes { get; set; } 
        public List<Note> PercussionNotes { get; set; }
        /// <summary>
        /// These are valid midi file chunks, built from the normalized data
        /// </summary>
        public List<TrackChunk> Tracks { get; set; }

        public List<MidiEvent> ChannelIndependentEvents { get; set; }


        public static string GetSongAsBase64EncodedMidi(string base64encodedMidi)
        {
            var normalita = new NormalizedSong(base64encodedMidi);
            return base64encodedMidi;
        }
    }
}
