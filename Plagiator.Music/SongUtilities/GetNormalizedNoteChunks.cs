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
        //public static List<TrackChunk> GetNormalizedNotesChunks(Song normalita)
        //{
        //    var dicky = new Dictionary<GeneralMidi2Program, TrackChunk>();
        //    // This dictionary assigns 1 instrument to each channel
        //    var channels = new Dictionary<GeneralMidi2Program, FourBitNumber>();
        //    for (byte i = 0; i < normalita.Instruments.Count; i++)
        //    {
        //        channels[normalita.Instruments[i]] = new FourBitNumber(i);
        //    }
        //    foreach (var note in normalita.Notes)
        //    {
        //        dicky[note.Instrument].Events.Add(
        //            new NoteOnEvent
        //            {
        //                Channel = channels[note.Instrument],
        //                DeltaTime = note.StartSinceBeginningOfSongInTicks,
        //                NoteNumber = new SevenBitNumber(note.Pitch),
        //                Velocity = new SevenBitNumber(note.Volume)
        //            });
        //        dicky[note.Instrument].Events.Add(
        //            new NoteOffEvent
        //            {
        //                Channel = channels[note.Instrument],
        //                DeltaTime = note.EndSinceBeginningOfSongInTicks,
        //                NoteNumber = new SevenBitNumber(note.Pitch)
        //            });
        //        foreach (var bendito in note.PitchBending)
        //        {
        //            dicky[note.Instrument].Events.Add(
        //            new PitchBendEvent
        //            {
        //                Channel = channels[note.Instrument],
        //                DeltaTime = bendito.TicksSiceBeginningOfSong,
        //                PitchValue = (ushort)bendito.Pitch
        //            });
        //        };
        //    }
        //    var retObj = new List<TrackChunk>();
        //    foreach (var key in dicky.Keys)
        //    {
        //        var events = ConvertAccumulatedTimeToDeltaTime(dicky[key]);
        //        var chunkito = new TrackChunk();
        //        foreach (var e in events) chunkito.Events.Add(e);
        //        retObj.Add(chunkito);
        //    }
        //    return retObj;
        //}
    }
}
