using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.Models;
using System.Collections.Generic;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        /// <summary>
        /// Used to create a standard midi file from a normalized song
        /// </summary>
        /// <param name="normalita"></param>
        /// <returns></returns>
        public static List<TrackChunk> GetNormalizedNotesChunks(NormalizedSong normalita)
        {
            var dicky = new Dictionary<GeneralMidi2Program, TrackChunk>();
            // This dictionary assigns 1 instrument to each channel
            var channels = new Dictionary<GeneralMidi2Program, FourBitNumber>();
            for (byte i = 0; i < normalita.Instruments.Count; i++)
            {
                channels[normalita.Instruments[i]] = new FourBitNumber(i);
            }
            foreach (var note in normalita.Notes)
            {
                dicky[note.Instrument].Events.Add(
                    new NoteOnEvent
                    {
                        Channel = channels[note.Instrument],
                        DeltaTime = note.StartSinceBeginningOSongInTicks,
                        NoteNumber = new SevenBitNumber(note.Pitch),
                        Velocity = new SevenBitNumber(note.Volume)
                    });
                dicky[note.Instrument].Events.Add(
                    new NoteOffEvent
                    {
                        Channel = channels[note.Instrument],
                        DeltaTime = note.EndSinceBeginnintOfSongInTicks,
                        NoteNumber = new SevenBitNumber(note.Pitch)
                    });
                foreach (var bendito in note.PitchBendingEvents)
                {
                    dicky[note.Instrument].Events.Add(
                    new PitchBendEvent
                    {
                        Channel = channels[note.Instrument],
                        DeltaTime = bendito.DeltaTime,
                        PitchValue = bendito.PitchValue
                    });
                };
            }
            var retObj = new List<TrackChunk>();
            foreach (var key in dicky.Keys)
            {
                var events = ConvertAccumulatedTimeToDeltaTime(dicky[key]);
                var chunkito = new TrackChunk();
                foreach (var e in events) chunkito.Events.Add(e);
                retObj.Add(chunkito);
            }
            return retObj;
        }
    }
}
