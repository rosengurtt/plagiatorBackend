using Melanchall.DryWetMidi.Core;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<TempoChange> GetTempoChanges(Song song)
        {
            var retObj = new List<TempoChange>();
            var setTempoEvents = GetEventsOfType(song.OriginalMidiBase64Encoded, MidiEventType.SetTempo);
            var TempoEvents = QuantizeTempos(ConvertDeltaTimeToAccumulatedTime(setTempoEvents));
            foreach (var te in TempoEvents)
            {
                retObj.Add(new TempoChange()
                        {
                            SongId = song.Id,
                            MicrosecondsPerQuarterNote = (int)te.MicrosecondsPerQuarterNote,
                            TicksSinceBeginningOfSong = te.DeltaTime
                        }
                    );
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
