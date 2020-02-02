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
    public class NormalizedSong
    {
        /// <summary>
        /// This is the original midi file, base64 encoded so it can be
        /// manipulated as a string instead of an array of bytes
        /// </summary>
        private string Base64encodedMidi { get; }
        public int TicksPerBeat { get; }
        public List<GeneralMidi2Program> Instruments { get; set; }

        public List<Note> Notes { get; set; }
        public List<Note> PercussionNotes { get; set; }
        public List<Bar> Bars { get; }


        public NormalizedSong(string base64encodedMidi)
        {
            Base64encodedMidi = base64encodedMidi;
            TicksPerBeat = MidiProcessing.GetTicksPerBeatOfSong(base64encodedMidi);
            Notes = MidiProcessing.GetNotesOfSong(base64encodedMidi);
            PercussionNotes = MidiProcessing.GetNotesOfSong(base64encodedMidi, true);
            Instruments = MidiProcessing.GetInstruments(Notes);

            Bars = MidiProcessing.GetBarsOfSong(Base64encodedMidi);
            _notesOfBar = new Dictionary<int, List<Note>>();
            _instrumentsOfBar = new Dictionary<int, List<GeneralMidi2Program>>();
            foreach(var bar in Bars)
            {
                bar.HasTriplets = MidiProcessing.BarHasTriplets(this, bar);
            }
            Notes = MidiProcessing.QuantizeNotes(this);
        }


       
        
        /// <summary>
        /// We use this as a cache to avoid recalculations
        /// </summary>
        private Dictionary<int,List<Note>>  _notesOfBar { get; set; }
        private Dictionary<int, List<GeneralMidi2Program>> _instrumentsOfBar { get; set; }

        public List<Note> NotesOfBar(Bar bar)
        {
            if (_notesOfBar.ContainsKey(bar.BarNumber))
                return _notesOfBar[bar.BarNumber];
            var retObj = new List<Note>();
            foreach (var n in Notes)
            {
                int barLengthInTicks = bar.TimeSignature.Numerator * TicksPerBeat;
                var barStart = bar.TicksFromBeginningOfSong;
                var noteStart = n.StartInTicks;
                var noteEnd = n.EndInTicks;
                var barEnd = bar.TicksFromBeginningOfSong + barLengthInTicks;
                if (barEnd < noteStart || noteEnd <= barStart) continue;
          
                if (!retObj.Contains(n))
                    retObj.Add(n);
            }
            _notesOfBar[bar.BarNumber]= retObj
                .OrderBy(x=>x.StartInTicks).ToList();
            return _notesOfBar[bar.BarNumber];
        }
      
        public List<GeneralMidi2Program> InstrumentsOfBar(Bar bar)
        {
            if (_instrumentsOfBar.ContainsKey(bar.BarNumber))
                return _instrumentsOfBar[bar.BarNumber];
            var retObj = new List<GeneralMidi2Program>();
            foreach (var n in Notes)
            {
                int barLengthInTicks = bar.TimeSignature.Numerator * TicksPerBeat;
                var barStart = bar.TicksFromBeginningOfSong;
                var noteStart = n.StartInTicks;
                var noteEnd = n.EndInTicks;
                var barEnd = bar.TicksFromBeginningOfSong + barLengthInTicks;
                if (barEnd >= noteStart || noteEnd >= barStart) continue;

                if (!retObj.Contains(n.Instrument))
                    retObj.Add(n.Instrument);
            }
            _instrumentsOfBar[bar.BarNumber] = retObj.OrderBy(x=>x).ToList();
            return _instrumentsOfBar[bar.BarNumber];
        }
   

        private Dictionary<GeneralMidi2Program,List<Note>> _tracks { get; set; }

        public List<Note> TrackOfInstrument(GeneralMidi2Program instr)
        {
            if (_tracks.ContainsKey(instr))
                return _tracks[instr];
            var retObj = new List<Note>();
            foreach (var n in Notes)
            {
                if (n.Instrument == instr)
                    retObj.Add(n);
            }
            _tracks[instr] = retObj.OrderBy(n => n.StartInTicks).ToList();
            return _tracks[instr];
        }
    }
}
