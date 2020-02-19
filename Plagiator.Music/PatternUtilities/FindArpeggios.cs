using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        public static Dictionary<Arpeggio, ArpeggioOccurrence> FindArpeggiosInSong(Song song)
        {
            var retObj = new Dictionary<Arpeggio, ArpeggioOccurrence>();
            foreach (var instr in song.Instruments)
            {
                var notes = song.Versions[0].NotesOfInstrument(instr);
                var cleanedNotes = RemoveBassNotes(notes).ToList();

                for (int i = 3; i < 50; i++)
                {
                    var arpis = FindArpeggioOfLength(cleanedNotes, i, song);
                    foreach (var key in arpis.Keys)
                    {
                        // Check if we got already that arpeggio from another instrument track
                        var arpi = retObj.Keys.Where(x => x.IsEqual(key)).FirstOrDefault();
                        if (arpi==null) retObj[key] = arpis[key];
                        else
                        {
                            retObj[arpi].Occurrences = retObj[arpi].Occurrences.Concat(arpis[key].Occurrences).ToList();
                        }

                    }
                    ;
                }
            }
            return retObj;
        }



        /// <summary>
        /// Removes bass notes played at the same time as other higher one.
        /// There may still be polyphony if a note starts playing when another hasn't finished
        /// but if 2 notes are played at the same time, the lower one is ignored
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static IEnumerable<Note> RemoveBassNotes(List<Note> notes)
        {
            var retObj = new List<Note>();
            var orderedNotes = notes.OrderBy(x => x.StartSinceBeginningOfSongInTicks)
                .ThenBy(y => y.Pitch).ToList();
            for (int i = 0; i < orderedNotes.Count() - 1; i++)
            {
                int j = 0;
                while (i + j < orderedNotes.Count() &&
                    orderedNotes[i + j].StartSinceBeginningOfSongInTicks == orderedNotes[i].StartSinceBeginningOfSongInTicks)
                    j += 1;
                if (i + j < orderedNotes.Count())
                {
                    yield return orderedNotes[i + j - 1];
                }
                i += (j - 1);
            }
        }

        /// <summary>
        /// Finds a repeating arpeggio of a specific length in a sequence of notes 
        /// The notes in the 'notes' parameter must be ordered by start time
        /// and there should not be 2 notes started at the same time
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static Dictionary<Arpeggio, ArpeggioOccurrence> FindArpeggioOfLength(List<Note> notes, int length, Song song)
        {
            var retObj = new Dictionary<Arpeggio, ArpeggioOccurrence>();
            for (int i = 0; i < notes.Count - length; i++)
            {
                var distances = new long[length];
                for (int j = 0; j < length; j++)
                {
                    if (i + j + length > notes.Count - 1) continue;
                    distances[j] = notes[i + j + length].StartSinceBeginningOfSongInTicks -
                        notes[i + j].StartSinceBeginningOfSongInTicks;
                    if (notes[i + j].Pitch != notes[i + j + length].Pitch) break;
                    if (!DistancesAreTheSame(distances)) break;
                    if (j < length - 1) continue;

                    // if we reached this point, it means there are 2 consecutive patterns
                    // with the same pitches played regularly

                    var arpegioNotes = notes.ToArray().Skip(i).Take(length).ToArray();
                    var arpeggioPitches = GetPitchPatternOfNotesSeq(arpegioNotes).ToList();
                    var arpegioRelativeTimes = GetRelativeDistancesBetweenConsecutiveNotes(notes.ToArray().Skip(i).Take(length + 1).ToList()).ToList();

                    var arpegito = new Arpeggio()
                    {
                        PitchPattern = new PitchPattern() {
                            PitchesRelativeToFirst = arpeggioPitches
                        },
                        RythmPattern = new RythmPattern()
                        {
                            RelativeDurations = arpegioRelativeTimes
                        }
                    };
                    var songInt = new SongInterval()
                    {
                        StartInTicksSinceBeginningOfSong = notes[i].StartSinceBeginningOfSongInTicks,
                        EndInTicksSinceBeginningOfSong = notes[i + length].EndSinceBeginningOfSongInTicks
                    };
                    var arpi = retObj.Keys.Where(x => x.IsEqual(arpegito)).FirstOrDefault();

                    var occurs = new List<SongInterval>();
                    occurs.Add(songInt);
                    if (arpi == null)
                    {
                        retObj[arpegito] = new ArpeggioOccurrence()
                        {
                            Occurrences = occurs,
                            SongVersion = song.Versions[0],
                            Arpeggio = arpegito
                        };
                    }
                    else
                    {
                        retObj[arpi].Occurrences.Add(songInt);
                    }
                }
            }
            return retObj;
        }


        /// <summary>
        /// Given a sequence of notes played consecutively (without simultaneous notes)
        /// returns a sequence of numbers that express the relative distances. For ex. if
        /// all notes are equidistant, the sequence is 1,1,1,...
        /// If the distance between the first and the second is twice the distance between
        /// the second and third, it returns 2,1...
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetRelativeDistancesBetweenConsecutiveNotes(List<Note> notes)
        {

            for (int i = 0; i < notes.Count - 1; i++)
            {
                var dif = notes[i + 1].StartSinceBeginningOfSongInTicks -
                    notes[i].StartSinceBeginningOfSongInTicks;
                yield return (int)dif;
            }
        }


        /// <summary>
        /// Checks that the integers in the array are all essentially the same value
        /// (that corresponds to a time measured in ticks between 2 notes)
        /// </summary>
        /// <param name="distances"></param>
        /// <returns></returns>
        private static bool DistancesAreTheSame(long[] distances)
        {
            // This value represents half a quarter (una corchea)
            // We consider that the distance between the corresponding notes of 2
            // consecutive arpeggios must be constant with a tolerance of this threshold 
            int tolerance = 48;
            for (int i = 0; i < distances.Length - 1; i++)
            {
                for (int j = 1; j < distances.Length; j++)
                {
                    if (Math.Abs(distances[i] - distances[j]) > tolerance)
                        return false;
                }
            }
            return true;
        }
        private static IEnumerable<int> GetPitchPatternOfNotesSeq(Note[] notes)
        {
            for (int i = 1; i < notes.Length; i++)
                yield return notes[i].Pitch - notes[0].Pitch;
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
