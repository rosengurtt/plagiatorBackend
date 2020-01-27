using Melanchall.DryWetMidi.Core;
using Plagiator.Music.Models;
using System.Collections.Generic;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<Bar> GetEmptyBarsOfSong(string base64encodedMidiFile)
        {
            List<Bar> retObj = new List<Bar>();
            var midiFile = MidiFile.Read(base64encodedMidiFile);

            var ticksPerBeat = GetTicksPerBeatOfSong(base64encodedMidiFile);
            var songDurationInTicks = GetSongDurationInTicks(base64encodedMidiFile);
            var timeSignatureEvents = GetEventsOfType(base64encodedMidiFile, MidiEventType.TimeSignature);
            var setTempoEvents = GetEventsOfType(base64encodedMidiFile, MidiEventType.SetTempo);
            timeSignatureEvents = ConvertDeltaTimeToAccumulatedTime(timeSignatureEvents);
            setTempoEvents = ConvertDeltaTimeToAccumulatedTime(setTempoEvents);

            //status
            TimeSignatureEvent currentTimeSignature = new TimeSignatureEvent
            {
                Numerator = 4,
                Denominator = 4
            };
            if (timeSignatureEvents.Count > 0)
                currentTimeSignature = (TimeSignatureEvent)timeSignatureEvents[0];

            SetTempoEvent currentTempo = new SetTempoEvent
            {
                MicrosecondsPerQuarterNote = 500000
            };
            if (setTempoEvents.Count > 0)
                currentTempo = (SetTempoEvent)setTempoEvents[0];
            int timeSigIndex = 0;
            int tempoIndex = 0;
            long currentTick = 0;

            while (currentTick < songDurationInTicks)
            {
                if (currentTick > songDurationInTicks - 5000)
                {

                }
                long timeOfNextTimeSignatureEvent = songDurationInTicks;
                if (timeSignatureEvents.Count - 1 > timeSigIndex)
                    timeOfNextTimeSignatureEvent = timeSignatureEvents[timeSigIndex + 1].DeltaTime;
                long timeOfNextSetTempoEvent = songDurationInTicks;
                if (setTempoEvents.Count - 1 > tempoIndex)
                    timeOfNextSetTempoEvent = setTempoEvents[tempoIndex + 1].DeltaTime;

                long lastTickOfBarToBeAdded = currentTimeSignature.Numerator * ticksPerBeat + currentTick;

                while ((lastTickOfBarToBeAdded <= timeOfNextTimeSignatureEvent && lastTickOfBarToBeAdded <= timeOfNextSetTempoEvent) ||
                    (lastTickOfBarToBeAdded > songDurationInTicks))
                {
                    var bar = new Bar()
                    {
                        TimeSignature = new TimeSignature
                        {
                            Numerator = currentTimeSignature.Numerator,
                            Denominator = currentTimeSignature.Denominator
                        },
                        Tempo = currentTempo.MicrosecondsPerQuarterNote,
                        TicksFromBeginningOfSong = currentTick
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

    }
}
