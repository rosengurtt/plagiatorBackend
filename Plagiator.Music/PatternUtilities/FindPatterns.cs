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

        public static Dictionary<Pattern, List<Occurrence>> FindPatternsOfTypeInSong(
            Song song,
            int version,
            PatternType patternType,
            int minLengthToSearch = 3,
            int maxLengthToSearch = 50)
        {
            var retObj = new Dictionary<Pattern, List<Occurrence>>();
            foreach (var instr in song.Instruments)
            {
                var notes = song.Versions[0].NotesOfInstrument(instr);
                var voicesNotes = GetVoices(notes);
                foreach (var voice in voicesNotes.Keys)
                {
                    var melody =  new Melody(voicesNotes[voice]);
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
                            var noteOfSongCorrespondingToFirstNote = FindNoteOfSong(firstNote, song, version, instr);
                            var patternLength = pat.Key.Split(",").Length;
                            var lastNote = melody.Notes[oc + patternLength];
                            var noteOfSongCorrespondingToLastNote = FindNoteOfSong(lastNote, song, version, instr);

                            var o = new Occurrence()
                            {
                                Pattern = patito,
                                FirstNoteId = noteOfSongCorrespondingToFirstNote.Id,
                                LastNoteId = noteOfSongCorrespondingToLastNote.Id,
                                SongVersionId = song.Versions[version].Id
                            };
                            ocur.Add(o);
                        }
                        retObj[patito] = ocur;
                    }
                }
              
            }
            return SimplifyPatterns(retObj);
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
            foreach(var note in notes)
            {
                if (!voicesNotes.Keys.Contains(note.Voice))
                    voicesNotes[note.Voice] = new List<Note>();
                voicesNotes[note.Voice].Add(note);
            }
            return voicesNotes;
        }

        private static Note FindNoteOfSong(Note note, Song song, int version, GeneralMidi2Program instr)
        {
            return song.Versions[version].Notes
                            .Where(n => n.Instrument == instr & n.Pitch == note.Pitch &
                            n.StartSinceBeginningOfSongInTicks == note.StartSinceBeginningOfSongInTicks &
                            n.EndSinceBeginningOfSongInTicks == note.EndSinceBeginningOfSongInTicks)
                            .FirstOrDefault();
        }


        public static Dictionary<Pattern, List<Occurrence>> SimplifyPatterns(Dictionary<Pattern, List<Occurrence>> patternsOc)
        {
            var patterns = patternsOc.Keys.ToList();

            var simplifiedPatterns = new Dictionary<Pattern, List<Occurrence>>();

            if (patterns.Count > 0 && patterns[0].PatternTypeId != PatternType.Pitch)
            {
                foreach (var pat in patterns)
                {
                    List<int> durationsAsIntegers;
                    RythmPattern patry = null;
                    MelodyPattern patme = null;
                    if (pat.PatternTypeId == PatternType.Rythm)
                    {
                         patry = new RythmPattern(pat);
                        durationsAsIntegers = patry.RelativeDurations;
                    }
                    else
                    {
                        patme = new MelodyPattern(pat);
                        durationsAsIntegers = patme.RelativeDurations;
                    }

                    // Simplify cases like 17,15,16 to 1,1,1
                    if (durationsAsIntegers.Min() > 4 && (durationsAsIntegers.Max() - durationsAsIntegers.Min() <= 2))
                    {   
                        var simplifiedPattern = new Pattern
                            {
                                AsString = "1",
                                PatternTypeId = PatternType.Rythm
                            };
                        if (pat.PatternTypeId == PatternType.Melody)
                        {
                            simplifiedPattern = new MelodyPattern(patme.DeltaPitches, new List<int>() { 1 })
                                .AsPattern;                        
                        }            
                        if (!simplifiedPatterns.Keys.Contains(simplifiedPattern))
                        {
                            simplifiedPatterns[simplifiedPattern] = new List<Occurrence>();
                        }
                        foreach (var oc in patternsOc[pat])
                        {
                            var clonito = oc.Clone();
                            clonito.Pattern = simplifiedPattern;
                            simplifiedPatterns[simplifiedPattern].Add(clonito);
                        }
                        continue;
                    }
                    // Divide by the maximum common divisor, so for ex 4,2,2 is converted to 2,1,1
                    var relativeDurations = new List<int>();
                    int gcd = GCD(durationsAsIntegers.ToArray());
                    for (int i = 0; i < durationsAsIntegers.Count; i++)
                        relativeDurations.Add(durationsAsIntegers[i] / gcd);
                    // If the pattern itself has a pattern, get the shortest pattern
                    // For ex instead of 2,1,2,1,2,1 we want just 2,1
                    relativeDurations = PatternUtilities.GetShortestPattern(relativeDurations);
                    var rythmPattern = new RythmPattern(relativeDurations);
                    var simplifiedPattern2 = new Pattern
                    {
                        AsString = rythmPattern.AsString,
                        PatternTypeId = PatternType.Rythm
                    };
                    if (pat.PatternTypeId == PatternType.Melody)
                    {
                        simplifiedPattern2 = new MelodyPattern(patme.DeltaPitches, relativeDurations)
                             .AsPattern;
                    }
                    if (!simplifiedPatterns.Keys.Contains(simplifiedPattern2))
                    {
                        simplifiedPatterns[simplifiedPattern2] = new List<Occurrence>();
                    }
                    foreach (var oc in patternsOc[pat])
                    {
                        var clonito = oc.Clone();
                        clonito.Pattern = simplifiedPattern2;
                        simplifiedPatterns[simplifiedPattern2].Add(clonito);
                    }
                }
                return simplifiedPatterns;
            }
            else
                return patternsOc;
        }
        
        private static List<List<int>> GetPatternsAsListOfDurations(List<Pattern> patterns)
        {
            var retObj = new List<List<int>>();
            foreach (var pat in patterns)
            {
                var durationsAsStrings = pat.AsString.Split(",").ToList();
                var durationsAsIntegers = ConvertToListOfIntegers(durationsAsStrings);
                retObj.Add(durationsAsIntegers);
            }
            return retObj;
        }
        public static List<int> GetShortestPattern(List<int> numbers)
        {
            var divisors = GetDivisorsOfNumber(numbers.Count).OrderByDescending(x => x);
            foreach (int j in divisors)
            {
                int lengthOfGroup = (int)(numbers.Count / j);
                int i = 0;
                int n = 1;
                while (i + n * lengthOfGroup < numbers.Count)
                {
                    if (numbers[i] != numbers[i + n * lengthOfGroup]) break;
                    i++;
                    if (i == lengthOfGroup)
                    {
                        i = 0;
                        n++;
                    }
                }
                if (i + n * lengthOfGroup == numbers.Count)
                {
                    return numbers.Take(lengthOfGroup).ToList();
                }
            }
            return numbers;
        }
        private static IEnumerable<int> GetDivisorsOfNumber(int number)
        {
            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0) yield return i;
            }
        }
        private static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        private static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }

}
