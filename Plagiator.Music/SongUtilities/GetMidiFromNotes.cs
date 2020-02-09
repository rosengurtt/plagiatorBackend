using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static string GetMidiFromNotes(Song song)
        {
            var mf = new MidiFile();
            mf.TimeDivision = new TicksPerQuarterNoteTimeDivision((short)song.TicksPerQuarterNote);

            var chunkito = new TrackChunk();
            var instrumentsChannels = new Dictionary<GeneralMidi2Program, byte>();
            foreach (var n in song.Notes.OrderBy(n => n.StartSinceBeginningOfSongInTicks))
            {
                if (n.IsPercussion)
                {
                    chunkito = AddEventsOfNote(chunkito, n, 9);
                }
                else
                {
                    if (!(instrumentsChannels.Keys).ToList().Contains(n.Instrument))
                    {
                        byte channel = GetFreeChannelForNote(song, instrumentsChannels, n);
                        instrumentsChannels[n.Instrument] = channel;
                        chunkito = AddProgramChangeEventToChunk(chunkito, channel,
                            n.StartSinceBeginningOfSongInTicks, (byte)n.Instrument);
                    }
                    chunkito = AddEventsOfNote(chunkito, n, instrumentsChannels[n.Instrument]);
                }
            }


            chunkito = AddSetTempoEvents(chunkito, song.TempoChanges);
            chunkito.Events._events = MidiProcessing.ConvertAccumulatedTimeToDeltaTime(chunkito.Events.OrderBy(x => x.DeltaTime).ToList());
            mf.Chunks.Add(chunkito);
            using (MemoryStream memStream = new MemoryStream(1000000))
            {
                mf.Write(memStream);
                var bytes = memStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        private static TrackChunk AddSetTempoEvents(TrackChunk chunkito, List<TempoChange> tc)
        {
            foreach(var t in tc)
            {
                var setTempoEvent = new SetTempoEvent()
                {
                    DeltaTime = t.TicksSinceBeginningOfSong,
                    MicrosecondsPerQuarterNote = t.MicrosecondsPerQuarterNote
                };
                chunkito.Events.Add(setTempoEvent);
            }
            return chunkito;
        }
        private static TrackChunk AddEventsOfNote(
            TrackChunk chunkito,
            Note n,
            byte channel)
        {
            var noteOn = new NoteOnEvent()
            {
                Channel = new FourBitNumber(channel),
                DeltaTime = n.StartSinceBeginningOfSongInTicks,
                NoteNumber = new SevenBitNumber(n.Pitch),
                Velocity = new SevenBitNumber(n.Volume)
            };
            chunkito.Events.Add(noteOn);
            var noteOff = new NoteOffEvent()
            {
                Channel = new FourBitNumber(channel),
                DeltaTime = n.EndSinceBeginningOfSongInTicks,
                NoteNumber = new SevenBitNumber(n.Pitch)
            };
            chunkito.Events.Add(noteOff);
            foreach(var pb in n.PitchBending)
            {
                var p = new PitchBendEvent()
                {
                    Channel = new FourBitNumber(channel),
                    DeltaTime = pb.TicksSinceBeginningOfSong,
                    PitchValue = pb.Pitch
                };
                chunkito.Events.Add(p);
            }
            return chunkito;
        }

        private static TrackChunk AddProgramChangeEventToChunk(
            TrackChunk chunkito, 
            byte channel, 
            long deltaTime, 
            byte instrument)
        {
            var programChange = new ProgramChangeEvent()
            {
                Channel = new FourBitNumber(channel),
                DeltaTime = deltaTime,
                ProgramNumber = new SevenBitNumber(instrument)
            };
            chunkito.Events.Add(programChange);
            return chunkito;
        }
        private static byte GetFreeChannelForNote(
            Song song, 
            Dictionary<GeneralMidi2Program, byte> instrumentsChannels, 
            Note n)
        {
            var busyChannels = new List<byte>();
            foreach(var noti in GetSimultaneousNotesOf(song, n))
            {
                foreach(var inst in instrumentsChannels.Keys)
                {
                    if (noti.Instrument == inst)
                        busyChannels.Add(instrumentsChannels[inst]);
                }
            }
            for (byte i = 0; i < 16; i++)
            {
                // Channel 9 is for percussion
                if (i == 9) continue;
                if (!busyChannels.Contains(i))
                {
                    return i;
                }
            }
            throw (new Exception("There are no enough channels for this song"));
        }
        private static List<Note> GetSimultaneousNotesOf(Song song, Note n)
        {
            var retObj = new List<Note>();
            foreach (var noti in song.Notes)
            {
                if ((noti.StartSinceBeginningOfSongInTicks < n.EndSinceBeginningOfSongInTicks
                    && noti.EndSinceBeginningOfSongInTicks >= n.EndSinceBeginningOfSongInTicks) ||
                    (n.StartSinceBeginningOfSongInTicks < noti.EndSinceBeginningOfSongInTicks
                    && n.EndSinceBeginningOfSongInTicks >= noti.EndSinceBeginningOfSongInTicks))
                    retObj.Add(noti);
            }
            return retObj;
        }
    }
}
