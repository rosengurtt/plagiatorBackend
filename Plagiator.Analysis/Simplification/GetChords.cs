using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis
{
    public static partial class SimplificationUtilities
    {
        public static List<Chord> GetChordsOfSimplification(SongSimplification simpl)
        {
            var durationOfHalfBeatInTicks = 48;
            var retObj = new List<Chord>();
            // We create a dictionary where the keys are points it time
            // one for every half beat of the song and the values are the notes 
            // played in that half beat
            // The assumption is that we don't have more than 2 chords in 1 beat
            // Since a note can last several beats, the same note and 
            // the same chord can be in many entries
            // of the dictionary
            var beatNotes = new Dictionary<long, List<Note>>();
            foreach (Note n in simpl.Notes)
            {
                if (n.IsPercussion) continue;
                var startBeat = n.StartSinceBeginningOfSongInTicks / durationOfHalfBeatInTicks;
                var endBeat = n.EndSinceBeginningOfSongInTicks / durationOfHalfBeatInTicks;
                for (var i = startBeat; i <= endBeat; i++)
                {
                    if (!beatNotes.ContainsKey(i)) beatNotes[i] = new List<Note>();
                    beatNotes[i].Add(n);
                }
            }
            // Now we look at the notes played in each half beat. 
            // We group the notes according to their start and stop times
            // The notes that start and stop simultatneously are 
            // candidates for a chord
            foreach (var slice in beatNotes.Keys)
            {
                var sliceNotes = beatNotes[slice];
                if (sliceNotes.Count < 2) continue;
                var possibleChords = new List<List<Note>>();
                foreach(var n in sliceNotes)
                {
                    var isNoteProcessed = false;
                    foreach(var possibleChord in possibleChords)
                    {
                        if (IsNoteSimultaneousWithGroup(possibleChord, n))
                        {
                            possibleChord.Add(n);
                            isNoteProcessed = true;
                        }
                    }
                    if (!isNoteProcessed)
                    {
                        var possibleChord = new List<Note>();
                        possibleChord.Add(n);
                        possibleChords.Add(possibleChord);
                    }
                }
                // We have now in possibleChords a list of groups of notes
                // We select the one that has more notes
                if (possibleChords.Count > 0)
                {
                    var maxNotesInAChord = possibleChords.Select(x => x.Count).Max();
                    if (maxNotesInAChord == 1) continue;
                    var theChord = possibleChords
                        .Where(x => x.Count == maxNotesInAChord).FirstOrDefault();
                    var newChord = new Chord(theChord);
                    if (NotesGenerateHarmony(theChord) && !IsChordInList(retObj, newChord))
                        retObj.Add(newChord);
                }
            }
            return retObj;
        }

        /// <summary>
        /// Rounds a number in integer multiples of precision
        /// For ex when number is 37 
        ///  - if precision = 1 returns 37
        ///  - if precision = 2 returns 36
        ///  - if precision = 5 returns 35
        ///  - if precision = 8 returns 40
        /// </summary>
        /// <param name="number"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static long RoundNumber(long number, long precision)
        {
            var quotient = number / precision;
            if (number % precision < 0.5 * (double)precision)
                return quotient * precision;
            else
                return (quotient + 1) * (precision);
        }

        private static bool AreNotesSimultaneous(Note n1, Note n2)
        {
            var start1 = n1.StartSinceBeginningOfSongInTicks;
            var start2 = n2.StartSinceBeginningOfSongInTicks;
            var end1 = n1.EndSinceBeginningOfSongInTicks;
            var end2 = n2.EndSinceBeginningOfSongInTicks;
            var minDuration = Math.Min(n1.DurationInTicks, n2.DurationInTicks);
            if (start1 > end2 || start2 > end1) return false;
            if (Math.Min(end1 - start2, end2 - start1) < minDuration / (double)2)
                return false;
            return true;
        }
        private static bool IsNoteSimultaneousWithGroup(List<Note> group, Note n)
        {
            var groupMinStart = group
                .Select(n => n.StartSinceBeginningOfSongInTicks).Min();
            var groupMaxStart = group
                .Select(n => n.StartSinceBeginningOfSongInTicks).Max();
            var groupMinEnd = group
                .Select(n => n.EndSinceBeginningOfSongInTicks).Min();
            var groupMaxEnd = group
                .Select(n => n.EndSinceBeginningOfSongInTicks).Max();
            if (Math.Abs(groupMinStart-n.StartSinceBeginningOfSongInTicks)>24 ||
                Math.Abs(groupMaxStart - n.StartSinceBeginningOfSongInTicks) > 24 ||
                Math.Abs(groupMinEnd - n.EndSinceBeginningOfSongInTicks) > 24 ||
                Math.Abs(groupMaxEnd - n.EndSinceBeginningOfSongInTicks) > 24)
                return false;
            return true;
        }
 
        /// <summary>
        /// If we have a group of notes, where they all have the same pitch
        /// they don't produce harmony. This method checks if a group of 
        /// notes have all the same pitch or not
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static bool NotesGenerateHarmony(List<Note> notes)
        {
            if (notes.Count < 2) return false;
            var firstNote = notes[0];
            foreach(var n in notes)
            {
                if (Math.Abs(n.Pitch - firstNote.Pitch) % 12 > 0) return true;
            }
            return false;
        }
    
        private static bool IsChordInList(List<Chord> list, Chord chord)
        {
            var matches = list
                .Where(x => x.Notes.Count == chord.Notes.Count && x.StartTick==chord.StartTick).ToList();
            if (matches.Count > 0) return true;
            return false;

        }
    }
}
