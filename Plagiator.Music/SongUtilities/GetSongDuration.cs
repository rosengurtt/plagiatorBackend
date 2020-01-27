using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {

        public static long GetSongDurationInTicks(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            long duration = 0;
            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                long trackDuration = 0;
                foreach (var eventito in chunk.Events)
                {
                    trackDuration += eventito.DeltaTime;
                }
                if (trackDuration > duration) duration = trackDuration;
            }
            return duration;
        }

        public static int GetSongDurationInSeconds(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            midiFile = ConvertDeltaTimeToAccumulatedTime(midiFile);
            int ticksPerBeat = ((TicksPerQuarterNoteTimeDivision)midiFile.TimeDivision).TicksPerQuarterNote;
            var totalDurationInTicks = GetSongDurationInTicks(base64encodedMidiFile);
            var tempoEvents = GetEventsOfType(midiFile, MidiEventType.SetTempo, true);
            var defaultTempo = 500000;
            if (tempoEvents == null || tempoEvents.Count == 0)
                return (int)Math.Ceiling(GetSeconds(totalDurationInTicks, ticksPerBeat, defaultTempo));
            double durationSoFar = GetSeconds(tempoEvents[0].DeltaTime, ticksPerBeat, defaultTempo);

            for (int i = 0; i < tempoEvents.Count; i++)
            {
                var durationInTicks = (i < tempoEvents.Count - 1) ?
                    tempoEvents[i + 1].DeltaTime - tempoEvents[i].DeltaTime :
                    totalDurationInTicks - tempoEvents[i].DeltaTime;
                var tempo = ((SetTempoEvent)tempoEvents[i]).MicrosecondsPerQuarterNote;

                var sacamela = GetSeconds(durationInTicks, ticksPerBeat, tempo);
                durationSoFar += GetSeconds(durationInTicks, ticksPerBeat, tempo);
            }
            return (int)Math.Ceiling(durationSoFar);
        }


        private static double GetSeconds(long ticks, long ticksPerBeat, long tempo)
        {
            return (ticks * tempo) / (ticksPerBeat * 1000000);
        }

        public static int GetTicksPerBeatOfSong(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            var ticksPerQuarter = midiFile.TimeDivision as TicksPerQuarterNoteTimeDivision;
            if (ticksPerQuarter != null) return ticksPerQuarter.TicksPerQuarterNote;
            else throw new Exception("The midi file doesn't have a value for TicksPerBeat");
        }


    }
}
