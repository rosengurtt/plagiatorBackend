using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Music.Models
{
    /// <summary>
    /// A melody is a monophonic sequence of notes 
    /// </summary>
    public class Melody
    {
        public List<Note> Notes { get; set; }

        /// <summary>
        /// Constructor
        /// The notes passed as parameter may have polyphony
        /// When there are 2 or moer notes playing at the same time, we keep the one with
        /// the highes pitch and remove the others
        /// </summary>
        /// <param name="notes"></param>
        public Melody(List<Note> notes)
        {
            Notes = RemoveBassNotes(notes).ToList();
        }

        public List<int> Pitches
        {
            get
            {
                var retObj = new List<int>();
                foreach (var note in Notes)
                    retObj.Add(note.Pitch);
                return retObj;
            }
        }
        public List<int> DurationsInTicks
        {
            get
            {
                var retObj = new List<int>();
                foreach (var note in Notes)
                    retObj.Add(note.DurationInTicks);
                return retObj;
            }
        }

        public List<string> DurationsInTicksAsStrings
        {
            get
            {
                return PatternUtilities.ConvertToListOfStrings(DurationsInTicks);
            }
        }
        public List<int> DeltaPitches
        {
            get
            {
                var retObj = new List<int>();
                retObj.Add(0);
                for (int i = 1; i < Pitches.Count(); i++)
                    retObj.Add(Pitches[i] - Pitches[i - 1]);
                return retObj;
            }
        }

        public List<string> DeltaPitchesAsStrings
        {
            get
            {
                return PatternUtilities.ConvertToListOfStrings(DeltaPitches);
            }
        }
        public IEnumerable<string> AsListOfStrings
        {
            get
            {
                return DeltaPitches.Zip(DurationsInTicks, (a, b) => $"({a}-{b})");
            }
        }


        private static IEnumerable<Note> RemoveBassNotes(List<Note> notes)
        {
            var orderedNotes = notes.OrderBy(x => x.StartSinceBeginningOfSongInTicks)
                .ThenBy(y => y.Pitch).ToList();
            for (int i = 0; i < orderedNotes.Count() - 1; i++)
            {
                int j = 0;
                while (i + j < orderedNotes.Count() &&
                    orderedNotes[i + j].StartSinceBeginningOfSongInTicks < orderedNotes[i].EndSinceBeginningOfSongInTicks &&
                    IntersectionComparedToDuration(orderedNotes[i + j], orderedNotes[i]) > 0.5)
                    j += 1;
                if (i + j < orderedNotes.Count())
                {
                    yield return orderedNotes[i + j - 1];
                }
                if (j > 1)
                    i += (j - 1);
            }
        }

        /// <summary>
        /// When we have 2 notes that sound at the same time, we want to keep the higher one and remove the other
        /// But it may be that 2 notes sound together for a short time due to imprecisions in the playing
        /// but they are distinct notes supposed to be played one after the other
        /// We discriminate between the 2 cases comparing the time they sound together with the average duration
        /// of the notes
        /// This function calculates that ratio
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        private static double IntersectionComparedToDuration(Note n1, Note n2)
        {
            var intersection = Math.Min(n1.EndSinceBeginningOfSongInTicks, n2.EndSinceBeginningOfSongInTicks) -
                Math.Max(n1.StartSinceBeginningOfSongInTicks, n2.StartSinceBeginningOfSongInTicks);
            var averageDuration = (n1.DurationInTicks + n2.DurationInTicks) / 2;
            return intersection / (double)averageDuration;
        }
    }
}
