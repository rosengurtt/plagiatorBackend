using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        public static Dictionary<Pattern, List<Occurrence>> FindPitchPatternsInSong(Song song, int version,
            GeneralMidi2Program instrument)
        {
            var maxLengthToSearch = 50;
            var minLengthToSearch = 3;
            var retObj = new Dictionary<Pattern, List<Occurrence>>();
            foreach (var instr in song.Instruments)
            {
                var notes = song.Versions[0].NotesOfInstrument(instr);
                var melody = new Melody(notes);
                var pitches = melody.Pitches.ToList();
                var patterns = FindPatternsInListOfIntegers(pitches, minLengthToSearch, maxLengthToSearch);
                foreach (var pat in patterns)
                {
                    var pithPat = new Pattern() { AsString = pat.Key, PatternType = PatternType.Pitch };
                    var ocur = new List<Occurrence>();
                    foreach (var oc in pat.Value)
                    {
                        var noteOfMelody = melody.Notes[oc];
                        var noteOfSong = song.Versions[version].Notes
                            .Where(n => n.Instrument == instrument & n.Pitch == noteOfMelody.Pitch &
                            n.StartSinceBeginningOfSongInTicks == noteOfMelody.StartSinceBeginningOfSongInTicks &
                            n.EndSinceBeginningOfSongInTicks == noteOfMelody.EndSinceBeginningOfSongInTicks)
                            .FirstOrDefault();
                        var o = new Occurrence() { Pattern = pithPat, Note = noteOfSong };
                        ocur.Add(o);
                    }
                    retObj[pithPat] = ocur;
                }
            }
            return retObj;
        }

    }
}
