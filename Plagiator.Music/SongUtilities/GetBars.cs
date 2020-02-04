using Melanchall.DryWetMidi.Core;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<Bar> GetBarsOfSong(string base64encodedMidiFile)
        {
            List<Bar> retObj = new List<Bar>();
            int barNumber = 1;
            var midiFile = MidiFile.Read(base64encodedMidiFile);

            var ticksPerBeat = GetTicksPerBeatOfSong(base64encodedMidiFile);
            var songDurationInTicks = GetSongDurationInTicks(base64encodedMidiFile);
            var timeSignatureEvents = GetEventsOfType(base64encodedMidiFile, MidiEventType.TimeSignature);
            var setTempoEvents = GetEventsOfType(base64encodedMidiFile, MidiEventType.SetTempo);
            timeSignatureEvents = ConvertDeltaTimeToAccumulatedTime(timeSignatureEvents);
            var TempoEvents = QuantizeTempos(ConvertDeltaTimeToAccumulatedTime(setTempoEvents));

            //status
            TimeSignatureEvent currentTimeSignature = new TimeSignatureEvent
            {
                Numerator = 4,
                Denominator = 4
            };
            if (timeSignatureEvents.Count > 0)
                currentTimeSignature = (TimeSignatureEvent)timeSignatureEvents[0];

            int currentTempo =  500000;

            int timeSigIndex = 0;
            int tempoIndex = 0;
            long currentTick = 0;

            while (currentTick < songDurationInTicks)
            {
                if (TempoEvents.Count > 0)
                    currentTempo =  (int)TempoEvents[tempoIndex].MicrosecondsPerQuarterNote;
                long timeOfNextTimeSignatureEvent = songDurationInTicks;
                if (timeSignatureEvents.Count - 1 > timeSigIndex)
                    timeOfNextTimeSignatureEvent = timeSignatureEvents[timeSigIndex + 1].DeltaTime;
                long timeOfNextSetTempoEvent = songDurationInTicks;
                if (TempoEvents.Count - 1 > tempoIndex)
                    timeOfNextSetTempoEvent = TempoEvents[tempoIndex + 1].DeltaTime;

                long lastTickOfBarToBeAdded = currentTimeSignature.Numerator * ticksPerBeat + currentTick;

                while ((lastTickOfBarToBeAdded <= timeOfNextTimeSignatureEvent && lastTickOfBarToBeAdded <= timeOfNextSetTempoEvent) ||
                    (lastTickOfBarToBeAdded > songDurationInTicks))
                {
                    var timeSignature = new TimeSignature
                    {
                        Numerator = currentTimeSignature.Numerator,
                        Denominator = currentTimeSignature.Denominator
                    };
                    var bar = new Bar
                    {
                        BarNumber = barNumber++,
                        TicksFromBeginningOfSong = currentTick,
                        TimeSignature = timeSignature,
                        TempoInMicrosecondsPerQuarterNote =currentTempo
                       
                    };   
                    retObj.Add(bar);
                    currentTick += currentTimeSignature.Numerator * ticksPerBeat;
                    lastTickOfBarToBeAdded += currentTimeSignature.Numerator * ticksPerBeat;
                    if (currentTick >= songDurationInTicks)
                        break;
                }
                if (lastTickOfBarToBeAdded >= timeOfNextTimeSignatureEvent)
                    timeSigIndex++;
                if (lastTickOfBarToBeAdded >= timeOfNextSetTempoEvent)
                    tempoIndex++;
            }
            return retObj;
        }

        /// <summary>
        /// Removes tempo changes that change the tempo by less than 15%
        /// </summary>
        /// <param name="evs"></param>
        /// <returns></returns>
        private static List<SetTempoEvent> QuantizeTempos(List<MidiEvent> evs)
        {
            int threshold = 15; // If the tempo change is less than 15%, ignore it
            var tempoEvs = new List<SetTempoEvent>();
            foreach (var ev in evs)
            {
                if (!(ev is SetTempoEvent)) continue;
                var evito = (SetTempoEvent)ev;
                tempoEvs.Add(evito);
            }
            if (tempoEvs.Count == 0) return null;
            var retObj = new List<SetTempoEvent>();
            retObj.Add(tempoEvs[0]);
            for (int i = 0; i < tempoEvs.Count - 1; i++)
            {
                var change = Math.Abs(tempoEvs[i].MicrosecondsPerQuarterNote -
                    tempoEvs[i + 1].MicrosecondsPerQuarterNote);
                var average = (tempoEvs[i].MicrosecondsPerQuarterNote +
                    tempoEvs[i + 1].MicrosecondsPerQuarterNote) / 2;
                var percentChange = (change / average) * 100;
                if (percentChange > threshold)
                    retObj.Add(tempoEvs[i + 1]);
            }
            return retObj;
        }
    }
}
