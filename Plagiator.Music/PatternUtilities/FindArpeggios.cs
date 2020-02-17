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
                    var arpegioRelativeTimes = GetRelativeDistancesBetweenConsecutiveNotes(notes.ToArray().Skip(i).Take(length + 1).ToList());

                    var arpegito = new Arpeggio()
                    {
                        PitchPattern = GetPitchPatternOfNotesSeq(arpegioNotes).ToList(),
                        RythmPattern = arpegioRelativeTimes
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
        private static List<int> GetRelativeDistancesBetweenConsecutiveNotes(List<Note> notes)
        {
            var retObj = new List<int>();

            for (int i = 0; i < notes.Count - 1; i++)
            {
                var dif = notes[i + 1].StartSinceBeginningOfSongInTicks -
                    notes[i].StartSinceBeginningOfSongInTicks;
                retObj.Add((int)dif);
            }
            return SimplifyListOfInts(retObj);
        }
        /// <summary>
        /// Tries to reduce the ints in a list, so for example if we have
        /// 4,4,2,4,8 it converts it to 2,2,1,2,4
        /// </summary>
        /// <param name="grandes"></param>
        /// <returns></returns>
        private static List<int> SimplifyListOfInts(List<int> grandes)
        {
            var retObj = new List<int>();
            // Simplify cases like 17,15,16 to 1,1,1
            if (grandes.Min() > 4)
            {
                if (grandes.Max() - grandes.Min() <= 2)
                    return Enumerable.Repeat(1, grandes.Count()).ToList();
            }

            int gcd = GCD(grandes.ToArray());
            for (int i = 0; i < grandes.Count; i++)
                retObj.Add(grandes[i] / gcd);
            return retObj;
        }
        static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
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


    }
}
