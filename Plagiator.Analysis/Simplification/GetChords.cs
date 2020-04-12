using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis
{
    public static partial class SimplificationUtilities
    {
        /// <summary>
        /// Looks for unique chords and all their occurrences in a song simplification
        /// The key of the dictionary returned is the chord expressed as a sequence
        /// of pitches separated by commas (like "40,46,54"). The pitches are sorted
        /// </summary>
        /// <param name="simpl"></param>
        /// <returns></returns>
        public static Dictionary<string, List<ChordOccurrence>> GetChordsOfSimplification(SongSimplification simpl)
        {
            var durationOfHalfBeatInTicks = 48;
            var retObj = new  Dictionary<string, List<ChordOccurrence>>();
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
                // We select the ones that have at least 2 notes
                if (possibleChords.Count > 0)
                {
                    foreach(var possibleChord in possibleChords)
                    {
                        if (possibleChord.Count < 2 ||
                            !NotesGenerateHarmony(possibleChord)) continue;
                        var thisChord = new Chord(possibleChord);
                        if (!retObj.ContainsKey(thisChord.PitchesAsString))
                            retObj[thisChord.PitchesAsString] = new List<ChordOccurrence>();
                        var chordOccurrence = new ChordOccurrence
                        {
                            StartTick = possibleChord.FirstOrDefault().StartSinceBeginningOfSongInTicks,
                            EndTick = possibleChord.FirstOrDefault().EndSinceBeginningOfSongInTicks,
                            SongSimplificationId = simpl.Id
                        };
                        retObj[thisChord.PitchesAsString].Add(chordOccurrence);
                    }
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

     
        private static bool IsNoteSimultaneousWithGroup(List<Note> group, Note n)
        {
            var groupStart = group.FirstOrDefault().StartSinceBeginningOfSongInTicks;
            var groupEnd = group.FirstOrDefault().EndSinceBeginningOfSongInTicks;
             
            if (groupStart==n.StartSinceBeginningOfSongInTicks &&
                groupEnd==n.EndSinceBeginningOfSongInTicks)
                return true;
            return false;
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
    
     
    }
}
