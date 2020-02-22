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
                var melody = new Melody(notes);
                List<int> elements = null;
                switch (patternType)
                {
                    case PatternType.Pitch:
                        elements = melody.Pitches.ToList();
                        break;
                    case PatternType.Rythm:
                        elements = melody.DurationsInTicks.ToList();
                        break;
                }
                var patterns = FindPatternsInListOfIntegers(elements, minLengthToSearch, maxLengthToSearch);
                foreach (var pat in patterns)
                {
                    var patito = new Pattern() { AsString = pat.Key, PatternTypeId = patternType };
                    var ocur = new List<Occurrence>();
                    foreach (var oc in pat.Value)
                    {
                        var noteOfMelody = melody.Notes[oc];
                        var noteOfSong = song.Versions[version].Notes
                            .Where(n => n.Instrument == instr & n.Pitch == noteOfMelody.Pitch &
                            n.StartSinceBeginningOfSongInTicks == noteOfMelody.StartSinceBeginningOfSongInTicks &
                            n.EndSinceBeginningOfSongInTicks == noteOfMelody.EndSinceBeginningOfSongInTicks)
                            .FirstOrDefault();
                        var o = new Occurrence() { Pattern = patito, Note = noteOfSong };
                        ocur.Add(o);
                    }
                    retObj[patito] = ocur;
                }
            }
            return retObj;
        }


        public static Dictionary<Pattern, List<Occurrence>> FindMelodyPatternsInSong(
           Dictionary<Pattern, List<Occurrence>> pitchPatterns,
           Dictionary<Pattern, List<Occurrence>> rythmPatterns)
        {
            var retObj = new Dictionary<Pattern, List<Occurrence>>();
            foreach (var pitchPat in pitchPatterns.Keys)
            {
                var pitchPatito = new PitchPattern(pitchPat);
                var lengthPitchPatito = pitchPatito.PitchesRelativeToFirst.Count + 1;
                foreach (var rythmPat in rythmPatterns.Keys)
                {
                    var rythmPatito = new RythmPattern(rythmPat);
                    var lengthRythmPatito = rythmPatito.RelativeDurations.Count;
                    if (lengthPitchPatito % lengthRythmPatito == 0 ||
                        lengthRythmPatito % lengthPitchPatito == 0)
                    {
                        var ocNotes = new List<Note>();
                        foreach (var ocpi in pitchPatterns[pitchPat])
                        {
                            foreach (var ocry in rythmPatterns[rythmPat])
                            {
                                if (ocpi.Note.IsEqual(ocry.Note))
                                {
                                    ocNotes.Add(ocpi.Note);
                                }
                            }
                        }
                        if (ocNotes.Count > 0)
                        {
                            var melPat = (new MelodyPattern(pitchPatito, rythmPatito)).AsPattern;
                            retObj[melPat] = new List<Occurrence>();
                            foreach (var oc in ocNotes)
                            {
                                retObj[melPat].Add(new Occurrence() { Note = oc, Pattern = melPat });
                            }
                        }
                    }
                }
            }
            return retObj;
        }


        /// <summary>
        /// When we have a repetitive sequence like 2,1,2,1,2,1 we want to keep just
        /// 2,1 and discard the rest
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
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
    }

}
