using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        //public static Dictionary<MelodyPattern, MelodyPatternOccurrence> FindMelodyPatternsInSong(Song song)
        //{
        //    var maxLengthToSearch = 50;
        //    var minLengthToSearch = 3;
        //    var retObj = new Dictionary<MelodyPattern, MelodyPatternOccurrence>();
        //    foreach (var instr in song.Instruments)
        //    {
        //        var notes = song.Versions[0].NotesOfInstrument(instr);
        //        var cleanedNotes = RemoveBassNotes(notes).ToList();

        //        // We start with longer lengths because if we have a pattern of lenght n, all subsequences
        //        // of shorter length are patterns as well, we want only the longer one
        //        for (int i = maxLengthToSearch; i > minLengthToSearch; i--)
        //        {
        //            var patternitos = FindMelodyPatternOfLength(cleanedNotes, i, song, retObj.Keys.ToList());
        //            foreach (var key in patternitos.Keys)
        //            {
        //                // Check if we got already that melody pattern from another instrument track
        //                var mp = retObj.Keys.Where(x => x.IsEqual(key)).FirstOrDefault();
        //                if (mp==null) retObj[key] = patternitos[key];
        //                else
        //                {
        //                    retObj[mp].Occurrences = retObj[mp].Occurrences.Concat(patternitos[key].Occurrences).ToList();
        //                }

        //            }
        //            ;
        //        }
        //    }
        //    return retObj;
        //}



        ///// <summary>
        ///// Removes bass notes played at the same time as other higher one.
        ///// There may still be polyphony if a note starts playing when another hasn't finished
        ///// but if 2 notes are played at the same time, the lower one is ignored
        ///// </summary>
        ///// <param name="notes"></param>
        ///// <returns></returns>
        //private static IEnumerable<Note> RemoveBassNotes(List<Note> notes)
        //{
        //    var retObj = new List<Note>();
        //    var orderedNotes = notes.OrderBy(x => x.StartSinceBeginningOfSongInTicks)
        //        .ThenBy(y => y.Pitch).ToList();
        //    for (int i = 0; i < orderedNotes.Count() - 1; i++)
        //    {
        //        int j = 0;
        //        while (i + j < orderedNotes.Count() &&
        //            orderedNotes[i + j].StartSinceBeginningOfSongInTicks == orderedNotes[i].StartSinceBeginningOfSongInTicks)
        //            j += 1;
        //        if (i + j < orderedNotes.Count())
        //        {
        //            yield return orderedNotes[i + j - 1];
        //        }
        //        i += (j - 1);
        //    }
        //}

        ///// <summary>
        ///// Finds a repeating melody pattern of a specific length in a sequence of notes 
        ///// The notes in the 'notes' parameter must be ordered by start time
        ///// and there should not be 2 notes started at the same time
        ///// </summary>
        ///// <param name="notes"></param>
        ///// <returns></returns>
        //private static Dictionary<MelodyPattern, MelodyPatternOccurrence> FindMelodyPatternOfLength(
        //    List<Note> notes, 
        //    int length, 
        //    Song song, 
        //    List<MelodyPattern> exclude)
        //{
        //    var retObj = new Dictionary<MelodyPattern, MelodyPatternOccurrence>();
        //    // Separation is the distance in consecutive notes between the end of the
        //    // first group of notes and the beginning of the second group
        //    for (int separation = 0; separation < notes.Count - 2 * length; separation++)
        //    {
        //        for (int i = 0; i < notes.Count - length; i++)
        //        {
        //            var distances = new long[length];
        //            for (int j = 0; j < length; j++)
        //            {
        //                if (i + j + length + separation > notes.Count - 1) continue;
        //                distances[j] = notes[i + j + length + separation].StartSinceBeginningOfSongInTicks -
        //                    notes[i + j].StartSinceBeginningOfSongInTicks;
        //                if (notes[i + j].Pitch != notes[i + j + length + separation].Pitch) break;
        //                if (!DistancesAreTheSame(distances)) break;
        //                if (j < length - 1) continue;

        //                // if we reached this point, it means we found a repeating pattern

        //                var melodyPatternNotes = notes.ToArray().Skip(i).Take(length).ToArray();
        //                var melodyPatternPitches = GetPitchPatternOfNotesSeq(melodyPatternNotes).ToList();
        //                var rythmRelativeTimes = GetRelativeDistancesBetweenConsecutiveNotes(notes.ToArray().Skip(i).Take(length + 1).ToList()).ToList();

        //                var patternito = new MelodyPattern()
        //                {
        //                    PitchPattern = new PitchPattern()
        //                    {
        //                        PitchesRelativeToFirst = melodyPatternPitches
        //                    },
        //                    RythmPattern = new RythmPattern()
        //                    {
        //                        RelativeDurations = rythmRelativeTimes
        //                    }
        //                };
        //                if (IsMelodyPatternIncludedInOneOfTheList(patternito, exclude)) break;

        //                var songInt = new SongInterval()
        //                {
        //                    StartInTicksSinceBeginningOfSong = notes[i].StartSinceBeginningOfSongInTicks,
        //                    EndInTicksSinceBeginningOfSong = notes[i + length].EndSinceBeginningOfSongInTicks
        //                };
        //                var arpi = retObj.Keys.Where(x => x.IsEqual(patternito)).FirstOrDefault();

        //                var occurs = new List<SongInterval>();
        //                occurs.Add(songInt);
        //                if (arpi == null)
        //                {
        //                    retObj[patternito] = new MelodyPatternOccurrence()
        //                    {
        //                        Occurrences = occurs,
        //                        SongVersion = song.Versions[0],
        //                        MelodyPattern = patternito
        //                    };
        //                }
        //                else
        //                {
        //                    retObj[arpi].Occurrences.Add(songInt);
        //                }
        //            }
        //        }
        //    }
        //    return retObj;
        //}

        //private static bool IsMelodyPatternIncludedInOneOfTheList(MelodyPattern mely, List<MelodyPattern> listy)
        //{
        //    foreach(var m in listy)
        //    {
        //        if (mely.Length > m.Length) continue;
        //        if (IsShortMelodyPatternInLargeMelodyPattern(mely, m)) return true;
        //    }
        //    return false;
        //}
        //private static bool IsShortMelodyPatternInLargeMelodyPattern(MelodyPattern shorty, MelodyPattern longy)
        //{
        //    var lengthShorty = shorty.RythmPattern.Length;
        //    var lengthLongy= longy.RythmPattern.Length;
        //    var shortyPitches = shorty.PitchPattern.PitchesRelativeToFirst;
        //    var longyPitches = longy.PitchPattern.PitchesRelativeToFirst;
        //    var shortyDurations = shorty.RythmPattern.RelativeDurations;
        //    var longyDurations = longy.RythmPattern.RelativeDurations;
        //    for (int n = 0; n < lengthLongy - lengthShorty; n++)
        //    {
        //        int i = 0;
        //        while (i < lengthShorty-1 && shortyPitches[i] == longyPitches[i + n]) i++;
        //        if (i < lengthShorty - 2) continue;
        //        i = 0;
        //        while (i < lengthShorty && shortyDurations[i] == longyDurations[i + n]) i++;
        //        if (i < lengthShorty - 1) continue;
        //        // if we reach this point, we found a match
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Given a sequence of notes played consecutively (without simultaneous notes)
        ///// returns a sequence of numbers that express the relative distances. For ex. if
        ///// all notes are equidistant, the sequence is 1,1,1,...
        ///// If the distance between the first and the second is twice the distance between
        ///// the second and third, it returns 2,1...
        ///// </summary>
        ///// <param name="notes"></param>
        ///// <returns></returns>
        //private static IEnumerable<int> GetRelativeDistancesBetweenConsecutiveNotes(List<Note> notes)
        //{

        //    for (int i = 0; i < notes.Count - 1; i++)
        //    {
        //        var dif = notes[i + 1].StartSinceBeginningOfSongInTicks -
        //            notes[i].StartSinceBeginningOfSongInTicks;
        //        yield return (int)dif;
        //    }
        //}


        ///// <summary>
        ///// Checks that the integers in the array are all essentially the same value
        ///// (that corresponds to a time measured in ticks between 2 notes)
        ///// </summary>
        ///// <param name="distances"></param>
        ///// <returns></returns>
        //private static bool DistancesAreTheSame(long[] distances)
        //{
        //    // This value represents half a quarter (una corchea)
        //    // We consider that the distance between the corresponding notes of 2
        //    // consecutive arpeggios must be constant with a tolerance of this threshold 
        //    int tolerance = 48;
        //    for (int i = 0; i < distances.Length - 1; i++)
        //    {
        //        for (int j = 1; j < distances.Length; j++)
        //        {
        //            if (Math.Abs(distances[i] - distances[j]) > tolerance)
        //                return false;
        //        }
        //    }
        //    return true;
        //}
        //private static IEnumerable<int> GetPitchPatternOfNotesSeq(Note[] notes)
        //{
        //    for (int i = 1; i < notes.Length; i++)
        //        yield return notes[i].Pitch - notes[0].Pitch;
        //}
        ///// <summary>
        ///// When we have a repetitive sequence like 2,1,2,1,2,1 we want to keep just
        ///// 2,1 and discard the rest
        ///// </summary>
        ///// <param name="numbers"></param>
        ///// <returns></returns>
        //public static List<int> GetShortestPattern(List<int> numbers)
        //{
        //    var divisors = GetDivisorsOfNumber(numbers.Count).OrderByDescending(x => x);
        //    foreach (int j in divisors)
        //    {
        //        int lengthOfGroup = (int)(numbers.Count / j);
        //        int i = 0;
        //        int n = 1;
        //        while (i + n * lengthOfGroup < numbers.Count)
        //        {
        //            if (numbers[i] != numbers[i + n * lengthOfGroup]) break;
        //            i++;
        //            if (i == lengthOfGroup)
        //            {
        //                i = 0;
        //                n++;
        //            }
        //        }
        //        if (i + n * lengthOfGroup == numbers.Count)
        //        {
        //            return numbers.Take(lengthOfGroup).ToList();
        //        }
        //    }
        //    return numbers;
        //}

        //private static IEnumerable<int> GetDivisorsOfNumber(int number)
        //{
        //    for (int i = 1; i <= number; i++)
        //    {
        //        if (number % i == 0) yield return i;
        //    }
        //}

    }
}
