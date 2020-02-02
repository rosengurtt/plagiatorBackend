﻿
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<Note> QuantizeNotes(NormalizedSong song)
        {
            var retObj = new List<Note>();
            foreach (var bar in song.Bars)
            {
                foreach (var n in song.NotesOfBar(bar))
                {
                    retObj.Add(QuantizeNote(n.Clone(), song.TicksPerBeat, bar.HasTriplets));
                }
            }
            return retObj;
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
        public static bool BarHasTriplets(NormalizedSong song, Bar bar)
        {
            var notes = song.NotesOfBar(bar);
            var lengthsOfTriplets = GetLengthsOfTriplets(song.TicksPerBeat);
            int numberOfTriplets = 0;
            foreach (var n in notes)
            {
                foreach (var q in lengthsOfTriplets) {
                    if (DurationIsEssentiallyTheSame(n, q, song.TicksPerBeat))
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
        private static bool DurationIsEssentiallyTheSame(Note n, int dur, int ticksPerBeat)
        {
            var dif = Math.Abs(n.DurationInTicks - dur);
            var avg = (n.DurationInTicks + dur) / 2;
            return (dif * 100 < 10);
        }
        /// <summary>
        /// Finds the valid durations of notes that have a duration
        /// that is 
        /// </summary>
        /// <param name="ticksPerBeat"></param>
        /// <returns></returns>
        private static List<int> GetLengthsOfTriplets(int ticksPerBeat)
        {
            var retObj = new List<int>();
            var min = ticksPerBeat / 3;
            for(int i = 0; i < 16; i++)
            {
                retObj.Add(min * i);
            }
            return retObj;
        }

        private static Note QuantizeNote(Note n, int ticksPerQuarterNote, bool hasTriplets)
        {
            if ((n.StartInTicks % ticksPerQuarterNote == 0) &&
                (n.EndInTicks % ticksPerQuarterNote == 0))
                return n;
            var retObj = n.Clone();
            retObj.StartInTicks = QuantizePointInTime(n.StartInTicks,
                n.DurationInTicks, ticksPerQuarterNote, hasTriplets);
            retObj.EndInTicks = QuantizePointInTime(n.EndInTicks,
             n.DurationInTicks, ticksPerQuarterNote, hasTriplets);
            return retObj;
        }

        private static long QuantizePointInTime(long point, int durationInTicks, int ticksPerQuarterNote, bool hasTriplets)
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
            var leftQuantizedPoint = point - point % quantum;
            var rightQuantizedPoint = leftQuantizedPoint + quantum;
            if (point - leftQuantizedPoint <= rightQuantizedPoint - point)
                return leftQuantizedPoint;
            return rightQuantizedPoint;
        }

    
    }
}