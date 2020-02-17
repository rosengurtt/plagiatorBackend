
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static IEnumerable<Note> QuantizeNotes(Song song, int songVersion)
        {
            int standardTicksPerQuarterNote = 96;

            foreach( var n in song.Versions[songVersion].Notes)
            {
                int i = 0;
                while (i < song.Bars.Count &&
                    song.Bars[i].TicksFromBeginningOfSong <= n.EndSinceBeginningOfSongInTicks)
                    i++;
                yield return QuantizeNote(n.Clone(), standardTicksPerQuarterNote, song.Bars[i-1].HasTriplets);

            }
        }

        /// <summary>
        /// We want to know if there are durations that are multiple of 3
        /// in the bar. If the bar has triplets, then when we quantize the
        /// notes in the bar we must aproximate points to values that
        /// are multiple of 3 and not only powers of 2
        /// </summary>
        /// <param name="song"></param>
        /// <param name="bar"></param>
        /// <returns></returns>
        public static bool BarHasTriplets(Song song, Bar bar)
        {
            int standardTicksPerQuarterNote = 96;
               var notes = song.NotesOfBar(bar, 0);
            var lengthsOfTriplets = GetLengthsOfTriplets(standardTicksPerQuarterNote);
            int numberOfTriplets = 0;
            foreach (var n in notes)
            {
                foreach (var q in lengthsOfTriplets) {
                    if (DurationIsEssentiallyTheSame(n, q))
                    {
                        numberOfTriplets++;
                        break;
                    }
                }
            }
            return (numberOfTriplets * 2 > notes.Count);
        }

        /// <summary>
        /// If the duration of a note in ticks is greater or smaller than
        /// an int by less than 10% we consider that the duration of the
        /// note can be the int, so it can be quantized to it
        /// </summary>
        /// <param name="n"></param>
        /// <param name="dur"></param>
        /// <param name="ticksPerBeat"></param>
        /// <returns></returns>
        private static bool DurationIsEssentiallyTheSame(Note n, int dur)
        {
            var dif = Math.Abs(n.DurationInTicks - dur);
            var avg = (n.DurationInTicks + dur) / 2;
            if (avg == 0) return true;
            return (dif * 100/avg < 10);
        }
        /// <summary>
        /// Finds the valid durations of notes that have a duration
        /// that is 
        /// </summary>
        /// <param name="ticksPerBeat"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetLengthsOfTriplets(int? ticksPerBeat)
        {
            var min = (int)ticksPerBeat / 3;
            for(int i = 0; i < 16; i++)
                yield return min * i;
        }

        private static Note QuantizeNote(Note n, int ticksPerQuarterNote, bool hasTriplets)
        {
            if ((n.StartSinceBeginningOfSongInTicks % ticksPerQuarterNote == 0) &&
                (n.EndSinceBeginningOfSongInTicks % ticksPerQuarterNote == 0))
                return n;
            var retObj = n.Clone();
            retObj.StartSinceBeginningOfSongInTicks = QuantizePointInTime(n.StartSinceBeginningOfSongInTicks,
                n.DurationInTicks, ticksPerQuarterNote, hasTriplets, true);
            retObj.EndSinceBeginningOfSongInTicks = QuantizePointInTime(n.EndSinceBeginningOfSongInTicks,
             n.DurationInTicks, ticksPerQuarterNote, hasTriplets, false);
            return retObj;
        }

        private static long QuantizePointInTime(long point, int durationInTicks, 
            int ticksPerQuarterNote, bool hasTriplets, bool pointIsStart)
        {
            // quantum represents the number of ticks that separate 2
            // consecutive quantized points  
            int quantum = (int)(hasTriplets ? Math.Round((double)durationInTicks / 6) :
                Math.Floor((double)durationInTicks / 4));

            if (quantum >= ticksPerQuarterNote)
                quantum = ticksPerQuarterNote;
            if (quantum == 0)
                quantum = 1;
            // that was a first aproximation. We want a quantum that divides the
            // tickPerQuarterNote value. So we find the first larger and the first
            // smaller values that divide ticksPerNote and we select the one closest
            // to our first aproximation
            int maxQuantum=quantum;
            while (ticksPerQuarterNote % maxQuantum != 0)
                maxQuantum++;
            int minQuantum = quantum;
            while ((ticksPerQuarterNote % minQuantum != 0) && minQuantum>1)
                minQuantum--;
            if (maxQuantum - quantum < quantum - minQuantum)
                quantum = maxQuantum;
            else
                quantum = minQuantum;

            // Now that we have the quantum, we find which of the left or right
            // points located on quantized values is closer 
            var shift = point % quantum;
            var leftQuantizedPoint = point - shift;
            var newDurationUsingLeft = pointIsStart ? durationInTicks + shift : durationInTicks - shift;
            var rightQuantizedPoint = leftQuantizedPoint + quantum;
            var newDurationUsingRight = pointIsStart ? durationInTicks - quantum + shift : durationInTicks + quantum - shift;

            if (point - leftQuantizedPoint < rightQuantizedPoint - point)
                return leftQuantizedPoint;
            else if (point - leftQuantizedPoint > rightQuantizedPoint - point)
                return rightQuantizedPoint;
            if (NumberOfWholeDivisors(newDurationUsingLeft) < NumberOfWholeDivisors(newDurationUsingRight))
                return rightQuantizedPoint;
            else return leftQuantizedPoint;
        }

        private static int NumberOfWholeDivisors(long number)
        {
            if (number == 0) return 0;
            int numberOfDivisors = 0;
            int i = 2;
            while (i < Math.Sqrt(number))
            {
                if (number % i == 0) numberOfDivisors++;
                i++;
            }
            return numberOfDivisors;
        }
    
    }
}
