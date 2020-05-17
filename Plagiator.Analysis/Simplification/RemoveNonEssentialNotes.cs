﻿using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Analysis
{
    static partial class SimplificationUtilities
    {

        /// <summary>
        /// Calculates how important is each note and removes a percentage of the notes as per
        /// the parameter percentageOfNotesToKeep, keeping the most important notes
        /// </summary>
        /// <param name="notes"></param>
        /// <param name="percentageOfNotesToKeep"></param>
        /// <returns></returns>
        public static List<Note> RemoveNonEssentialNotes(List<Note> notes, int percentageOfNotesToKeep)
        {
            var retObj = new List<Note>();
            var importance = 100;
            var totalNotesToKeep = (int)Math.Round(notes.Count() * percentageOfNotesToKeep / (double)100);
            var tolerance = totalNotesToKeep / 100;
            var voices = Utilities.GetVoices(notes);
            foreach (var voice in voices.Keys)
            {
                var voiceNotes = voices[voice].OrderBy(n => n.StartSinceBeginningOfSongInTicks).ToList();

                var notesImportance = voiceNotes.Select(x => GetNoteImportance(x, voiceNotes)).ToList();
                var totalNotesLeftWithCurrentImportance = TotalNotesLeft(voiceNotes, notesImportance, importance);

                // To avoid entering a loop or calculating twice the same, we keep the
                // calculations for different importance values in a dictionary, where the
                // key is the importance and the value is the number of corresponging notes 
                var computedImportances = new Dictionary<int, int>();
                computedImportances[importance] = totalNotesLeftWithCurrentImportance;
                var iteration = 1;
                while (Math.Abs(totalNotesLeftWithCurrentImportance - totalNotesToKeep) > tolerance)
                {
                    if (totalNotesLeftWithCurrentImportance > totalNotesToKeep)
                    {
                        var difference = (totalNotesLeftWithCurrentImportance - totalNotesToKeep) / (double)totalNotesToKeep;
                        importance += Math.Max(1, (int)Math.Round(20 * difference / Math.Sqrt(iteration++)));
                    }
                    else
                    {
                        var difference = (totalNotesToKeep - totalNotesLeftWithCurrentImportance) / (double)totalNotesToKeep;
                        importance -= Math.Max(1, (int)Math.Round(20 * difference / Math.Sqrt(iteration++)));
                    }
                    if (computedImportances.ContainsKey(importance)) break;
                    totalNotesLeftWithCurrentImportance = TotalNotesLeft(voiceNotes, notesImportance, importance);
                    computedImportances[importance] = totalNotesLeftWithCurrentImportance;
                }
                importance = GetBestImportanceValue(computedImportances, totalNotesToKeep);
                foreach (var n in voiceNotes.Where((x, i) => notesImportance[i] > importance))
                    retObj.Add(n);
            }
            return retObj;
        }

        private static int GetBestImportanceValue(Dictionary<int,int> computedImportances, int totalNotesToKeep)
        {
            var minDiff = 1000000;
            var bestImportance = 0;
            foreach(var imp in computedImportances.Keys)
            {
                if (Math.Abs(computedImportances[imp]- totalNotesToKeep)< minDiff)
                {
                    minDiff = Math.Abs(computedImportances[imp] - totalNotesToKeep);
                    bestImportance = imp;
                }
            }
            return bestImportance;
        }

        /// <summary>
        /// Returns the number of notes that will be kept if we remove the notes with an
        /// importance that is less than the imporatnce parameter passed
        /// </summary>
        /// <param name="notes"></param>
        /// <param name="importance"></param>
        /// <returns></returns>
        private static int TotalNotesLeft(List<Note> notes, List<int> notesImportance, int importance)
        {
            var notesLeft = notes.Where((x, i) => notesImportance[i] >= importance);
            return notesLeft.Count();
        }


        /// <summary>
        /// Calculates an integer between 0 and 200 that gives an idea of the importance of a note
        /// The things that give importance to a note are:
        /// - the duration
        /// - the volume
        /// - the location in a strong beat
        /// - if it is the highest pitch or the lowest pitch (if there are several notes
        ///   playing simultaneously, we hear more the hightest and the lowest pitch
        /// - if the pitch is very different of the notes playing near the note
        ///   
        /// Because the duration and the volume are relative to the surrounding notes
        /// we pass not only the note but the notes context as well.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        private static int GetNoteImportance(Note n, List<Note> notes)
        {
            var importance = 0;
            var relativeImportanceDuration = 80;
            var relativeImportanceVolume = 78;
            var relativeImportanceLocationInStrongBeat = 14;
            var relativeImportanceIsBassOrHigh = 14;
            var relativeImportancePitchIsDifferent = 14;

            // closeNeighboors are notes playing up to 4 quarter notes before or after
            var closeNeighboors = notes.Where(x =>
             Math.Abs(x.StartSinceBeginningOfSongInTicks - n.StartSinceBeginningOfSongInTicks) < 96 * 4
             || Math.Abs(x.EndSinceBeginningOfSongInTicks - n.EndSinceBeginningOfSongInTicks) < 96 * 4)
                .ToList();
            // farNeighboors are notes playing up to 12 quarter notes before or after
            var farNeighboors= notes.Where(x =>
              Math.Abs(x.StartSinceBeginningOfSongInTicks - n.StartSinceBeginningOfSongInTicks) < 96 * 12
              || Math.Abs(x.EndSinceBeginningOfSongInTicks - n.EndSinceBeginningOfSongInTicks) < 96 * 12)
                .ToList();
            var averageDurationOfCloseNeighboors = closeNeighboors.Select(x => x.DurationInTicks).Average();
            var averageVolumeOfCloseNeighboors = closeNeighboors.Select(x => (int)x.Volume).Average();
            var averageDurationOfFarNeighboors = farNeighboors.Select(x => x.DurationInTicks).Average();
            var averageVolumeOfFarNeighboors = farNeighboors.Select(x => (int)x.Volume).Average();

            // We give 70% importance to close neighboors and 30% to far neighboors
            var averageDurationOfNeighboors = 0.7 * averageDurationOfCloseNeighboors + 0.3 * averageDurationOfFarNeighboors;
            var averageVolumeOfNeighboors = 0.7 * averageVolumeOfCloseNeighboors + 0.3 * averageVolumeOfFarNeighboors;

            importance += (int)Math.Floor(sigmoid(n.DurationInTicks / averageDurationOfNeighboors) * relativeImportanceDuration);
            importance += (int)Math.Floor(sigmoid(n.Volume / averageVolumeOfNeighboors) * relativeImportanceVolume);

            if (IsInStrongBeat(n)) importance += relativeImportanceLocationInStrongBeat;
            if (IsBassOrHigh(n, notes)) importance += relativeImportanceIsBassOrHigh;
            if (ClosestInteval(n, closeNeighboors) > 12) importance += relativeImportancePitchIsDifferent;

            return importance;
        }

        private static double sigmoid(double x)
        {
            if (x > 3) return 1;
            if (x > 2) return 0.9 + 0.1 * (x - 2);
            if (x > 1.5) return 0.8 + 0.2 * (x - 1.5);
            if (x > 1) return 0.5 + 0.6 * (x - 1);
            if (x > 0.1) return 0.1 + (4 / (double)9) * (x - 0.1);
            return 0.1;
        }

        private static bool IsInStrongBeat(Note n)
        {
            return (n.StartSinceBeginningOfSongInTicks % 96 < 5 ||
                n.StartSinceBeginningOfSongInTicks % 96 > 90);
        }

        private static bool IsBassOrHigh(Note n, List<Note> notes)
        {
            var simultaneousNotes = notes.Where(x => x.StartSinceBeginningOfSongInTicks < n.EndSinceBeginningOfSongInTicks &&
              x.EndSinceBeginningOfSongInTicks > n.StartSinceBeginningOfSongInTicks).ToList();

            if (!simultaneousNotes.Where(x => x.Pitch > n.Pitch).Any()) return true;
            if (!simultaneousNotes.Where(x => x.Pitch < n.Pitch).Any()) return true;
            return false;
        }
   
        // Returns the internval in semitones of the note in the list notes
        // that has the closest pitch to n
        // Used to see if the pitch of a note is very different of all surrounding notes
        private static int ClosestInteval(Note n, List<Note> notes)
        {
            return notes.Select(x => Math.Abs(x.Pitch - n.Pitch)).Min();
        }
    }
}
