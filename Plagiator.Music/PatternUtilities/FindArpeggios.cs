﻿using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Music
{
    public static partial class PatternUtilities
    {
        public static List<Arpeggio> FindArpegiosInSong(Song song)
        {           
            var retObj = new List<Arpeggio>();
            foreach (var instr in song.Instruments)
            {
                var notes = song.Versions[0].NotesOfInstrument(instr);
                notes = RemoveBassNotes(notes);
                for(int i=3; i < 50;i++)
                {
                    var arpis = FindArpeggioOfLength(notes, i);
                    if (arpis.Count > 0)
                        retObj = retObj.Concat(arpis).ToList();
;                }     
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
        private static List<Note> RemoveBassNotes(List<Note> notes)
        {
            var retObj = new List<Note>();
            var orderedNotes = notes.OrderBy(x => x.StartSinceBeginningOfSongInTicks)
                .ThenBy(y => y.Pitch);
            for (int i = 0; i < orderedNotes.Count() - 1; i++)
            {
                int j = 0;
                while (i + j < orderedNotes.Count() &&
                    notes[i + j].StartSinceBeginningOfSongInTicks == notes[i].StartSinceBeginningOfSongInTicks)
                    j += 1;
                if (i + j < orderedNotes.Count())
                    retObj.Add(notes[i + j]);
            }
            return retObj;
        }

        /// <summary>
        /// Finds a repeating arpeggio of a specific length in a sequence of notes 
        /// The notes in the 'notes' parameter must be ordered by start time
        /// and there should not be 2 notes started at the same time
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static List<Arpeggio> FindArpeggioOfLength(List<Note> notes, int length)
        {
            var retObj = new List<Arpeggio>();
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
                        PitchPattern = GetPitchPatternOfNotesSeq(arpegioNotes),
                        RythmPattern = arpegioRelativeTimes,
                        Occurrences=new List<SongInterval>()

                    };
                    var songInt = new SongInterval()
                    {
                        StartInTicksSinceBeginningOfSong = notes[i].StartSinceBeginningOfSongInTicks,
                        EndInTicksSinceBeginningOfSong = notes[i + length].EndSinceBeginningOfSongInTicks
                    };
                    var arpi = retObj.Where(x => x.IsEqual(arpegito)).FirstOrDefault();
                    if (arpi == null)
                    {

                        arpegito.Occurrences.Add(songInt);
                        retObj.Add(arpegito);
                    }
                    else
                    {
                        arpi.Occurrences.Add(songInt);
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

            for (int i=0; i< notes.Count-1; i++)
            {
                var dif =notes[i + 1].StartSinceBeginningOfSongInTicks -
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
            int gcd = GCD(grandes.ToArray());
            if (gcd == 0)
            {

            }
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
            for (int i=0; i < distances.Length-1; i++)
            {
                for (int j=1; j < distances.Length; j++)
                {
                    if (Math.Abs(distances[i] - distances[j]) > tolerance)
                        return false;
                }
            }
            return true;
        }
        private static List<int> GetPitchPatternOfNotesSeq(Note[] notes)
        {
            var retObj = new List<int>();
            for (int i=1; i < notes.Length; i++)
            {
                retObj.Add(notes[i].Pitch - notes[0].Pitch);
            }
            return retObj;
        }
        private static List<int> GeIntervalsOfNotesSeq(Note[] notes)
        {
            var retObj = new List<int>();
            for (int i = 0; i < notes.Length - 1; i++)
            {
                for (int j = i + 1; j < notes.Length; j++)
                {
                    if (!retObj.Contains((j - i + 48) % 12))
                        retObj.Add((j - i + 48) % 12);
                }
            }
            return retObj;
        }

    }
}
