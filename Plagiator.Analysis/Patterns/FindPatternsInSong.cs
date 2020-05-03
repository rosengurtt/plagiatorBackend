using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis.Patterns
{
    public static partial class PatternUtilities
    {
        public static Dictionary<Pattern, List<Occurrence>> FindPatternsOfTypeInSong(
        Song song,
        int version,
        PatternType patternType,
        int minLengthToSearch = 3,
        int maxLengthToSearch = 12)
        {
            var retObj = new Dictionary<Pattern, List<Occurrence>>();
            foreach (var instr in song.SongStats.Instruments)
            {
                var notes = song.SongSimplifications[version].NotesOfInstrument(instr);
                var voicesNotes = GetVoices(notes.ToList());
                foreach (var voice in voicesNotes.Keys)
                {
                    var melody = new Melody(voicesNotes[voice]);
                    var elements = new List<string>();
                    switch (patternType)
                    {
                        case PatternType.Pitch:
                            elements = melody.DeltaPitchesAsStrings;
                            break;
                        case PatternType.Rythm:
                            elements = melody.DurationsInTicksAsStrings;
                            break;
                        case PatternType.Melody:
                            elements = melody.AsListOfStrings.ToList();
                            break;
                    }
                    var patterns = FindPatternsInListOfStrings(elements, minLengthToSearch, maxLengthToSearch);
                    foreach (var pat in patterns)
                    {
                        var patito = new Pattern() { AsString = pat.Key, PatternTypeId = patternType };
                        var ocur = new List<Occurrence>();
                        foreach (var oc in pat.Value)
                        {
                            var firstNote = melody.Notes[oc];
                            var patternLength = pat.Key.Split(",").Length;
                            var ocNotes = melody.Notes.ToArray()[oc..(oc + patternLength)].ToList();
                            var o = new Occurrence()
                            {
                                Pattern = patito,
                                Notes = ocNotes,
                                SongSimplificationId = song.SongSimplifications[version].Id
                            };
                            ocur.Add(o);
                        }
                        retObj[patito] = ocur;
                    }
                }

            }
            return SimplifyPatternsOcurrences(retObj);
        }

        /// <summary>
        /// If we have a group of notes from a song, all played with the same instrument,
        /// but corresponding to different voices (or tracks), this method splits the list
        /// in as many voices, one list per voice.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static Dictionary<int, List<Note>> GetVoices(List<Note> notes)
        {
            var voicesNotes = new Dictionary<int, List<Note>>();
            foreach (var note in notes)
            {
                if (!voicesNotes.Keys.Contains(note.Voice))
                    voicesNotes[note.Voice] = new List<Note>();
                voicesNotes[note.Voice].Add(note);
            }
            return voicesNotes;
        }

        private static Note FindNoteOfSong(Note note, Song song, int version, int instr)
        {
            return song.SongSimplifications[version].Notes
                            .Where(n => n.Instrument == instr & n.Pitch == note.Pitch &
                            n.StartSinceBeginningOfSongInTicks == note.StartSinceBeginningOfSongInTicks &
                            n.EndSinceBeginningOfSongInTicks == note.EndSinceBeginningOfSongInTicks)
                            .FirstOrDefault();
        }


        public static Dictionary<Pattern, List<Occurrence>> SimplifyPatternsOcurrences(Dictionary<Pattern, List<Occurrence>> patternsOc)
        {
            var patterns = patternsOc.Keys.ToList();

            var simplifiedPatternsOcurrences = new Dictionary<Pattern, List<Occurrence>>();
            foreach (var pat in patterns)
            {
                var patSimplified = SimplifyPattern(pat);
                if (!simplifiedPatternsOcurrences.Keys.Contains(patSimplified))
                {
                    simplifiedPatternsOcurrences[patSimplified] = new List<Occurrence>();
                }
                foreach (var oc in patternsOc[pat])
                    {
                        var clonito = oc.Clone();
                        clonito.Pattern = patSimplified;
                        simplifiedPatternsOcurrences[patSimplified].Add(clonito);
                    }
            }
            return simplifiedPatternsOcurrences;            
        }

    }
}
