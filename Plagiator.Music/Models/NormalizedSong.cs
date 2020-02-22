using System;
using System.Collections.Generic;
using System.Text;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.SongUtilities;
using System.Linq;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// The idea of this class is to have an object that is easy to manipulate
    /// for the purpose of composing
    /// 
    /// The changes we do to the original midi file are:
    /// 
    /// - All time info is always ticks from beginning of song instead of a delta
    ///   related to a previous event
    /// 
    /// - For each note played in the song, we have 
    ///     + the complete information of when it was played 
    ///     + the volume (extracted from the velocity midi value), 
    ///     + we take in consideration the sustain pedal by extending the time of the note
    ///       and removing the pedal event
    ///     + the pitch bending info 
    /// 
    /// - We keep the tempo changes that make significant changes to the tempo
    ///   (more than 10%) and discard the rest
    ///   
    /// - We keep the time signature changes, since they affect the bars
    /// 
    /// - We discard other effects, as aftertouch, pedals (except sustain, that
    ///   we use to calculate the duration of notes), etc.
    /// 
    /// - For bars and tracks, we build that data from the notes info
    ///   and we cache it to not have to recalculate it again, but
    ///   if there are note changes, the cache is discarded
    /// </summary>
    /// <param name="base64encodedMidi"></param>
    public partial class Song
    {        
        
       // public List<Note> Notes { get; set; }
       public List<SongVersion> Versions { get; set; }
        public List<Bar> Bars { get; set; }

        public List<TempoChange> TempoChanges { get; set; }

        public void NormalizeSong()
        {
            _notesOfBar = new Dictionary<int, List<Note>>();
            Versions = new List<SongVersion>();
            _instruments = new List<GeneralMidi2Program>();

            Versions.Add(new SongVersion()
            {
                Notes = MidiProcessing.GetNotesOfSong(OriginalMidiBase64Encoded),
                VersionNumber = 0
            });

            Bars = MidiProcessing.GetBarsOfSong(OriginalMidiBase64Encoded);

            foreach(var bar in Bars)
            {
                bar.HasTriplets = MidiProcessing.BarHasTriplets(this, bar);
            }
            Versions[0].Notes = MidiProcessing.QuantizeNotes(this, 0).ToList();

            Versions[0].Occurrences = new List<Occurrence>();
            var pitchPatterns = PatternUtilities.FindPatternsOfTypeInSong(this, 0, PatternType.Pitch);
            foreach (var p in pitchPatterns.Keys)
            {
                Versions[0].Occurrences = Versions[0].Occurrences.Concat(pitchPatterns[p]).ToList();
            }
            var rythmPatterns = PatternUtilities.FindPatternsOfTypeInSong(this, 0, PatternType.Rythm);
            foreach (var p in rythmPatterns.Keys)
            {
                Versions[0].Occurrences = Versions[0].Occurrences.Concat(rythmPatterns[p]).ToList();
            }
            var melodyPatterns = PatternUtilities.FindMelodyPatternsInSong(pitchPatterns, rythmPatterns);
            foreach (var p in melodyPatterns.Keys)
            {
                Versions[0].Occurrences = Versions[0].Occurrences.Concat(melodyPatterns[p]).ToList();
            }

            TempoChanges = MidiProcessing.GetTempoChanges(this);
            ProcessedMidiBase64Encoded = MidiProcessing.GetMidiFromNotes(this, 0);
        }


        /// <summary>
        /// We use this as a cache to avoid recalculations
        /// </summary>
        private Dictionary<int, List<Note>> _notesOfBar { get; set; }
        private Dictionary<int, List<GeneralMidi2Program>> _instrumentsOfBar { get; set; }

        
        public List<Note> NotesOfBar(Bar bar, int songVersion)
        {
            int standardTicksPerQuarterNote = 96;
            if (_notesOfBar.ContainsKey(bar.BarNumber))
                return _notesOfBar[bar.BarNumber];
            var retObj = new List<Note>();
            foreach (var n in Versions[songVersion].Notes)
            {
                int barLengthInTicks = bar.TimeSignature.Numerator * (int)standardTicksPerQuarterNote;
                var barStart = bar.TicksFromBeginningOfSong;
                var noteStart = n.StartSinceBeginningOfSongInTicks;
                var noteEnd = n.EndSinceBeginningOfSongInTicks;
                var barEnd = bar.TicksFromBeginningOfSong + barLengthInTicks;
                if (barEnd < noteStart || noteEnd <= barStart) continue;

                if (!retObj.Contains(n))
                    retObj.Add(n);
            }
            _notesOfBar[bar.BarNumber] = retObj
                .OrderBy(x => x.StartSinceBeginningOfSongInTicks).ToList();
            return _notesOfBar[bar.BarNumber];
        }

        private List<GeneralMidi2Program> _instruments { get; set; }
        public List<GeneralMidi2Program> Instruments
        {
            get
            {
                if (_instruments.Count==0)
                {
                    _instruments = MidiProcessing.GetInstruments(this.Versions[0].Notes);
                }
                return _instruments;
            }
        }

    }
}
