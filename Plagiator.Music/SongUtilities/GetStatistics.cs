using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {

        public static Song ComputeSongStats(Song song)
        {
            try
            {
                var midiFile = MidiFile.Read(song.OriginalMidiBase64Encoded);

                song.InitializeStats();
                song.TotalChunks = midiFile.Chunks.Count;
                song.DurationInSeconds = GetSongDurationInSeconds(song.OriginalMidiBase64Encoded);
                song.TimeSignature = GetMainTimeSignatureOfSong(song.OriginalMidiBase64Encoded);
                song.NumberBars = GetBarsOfSong(song.OriginalMidiBase64Encoded).Count;
                var channels = new List<FourBitNumber>();
                var pitches = new List<SevenBitNumber>();
                var uniquePitches = new List<int>();
                var instruments = new List<int>();
                var percussionInstruments = new List<int>();
                var tempoChanges = new List<int>();
                long songDurationInTicks = 0;
                var highestPitch = 0;
                var lowestPitch = 127;

                foreach (TrackChunk chunk in midiFile.Chunks)
                {
                    long chunkDurationInTicks = 0;
                    bool hasProgramChangeEvent = false;
                    var chunkNoteEventsWithNullDeltas = 0;
                    var chunkNoteEvents = 0;
                    var chunkIsDrumChannel = false;
                    var channelsInChunk = new List<FourBitNumber>();
                    foreach (var eventito in chunk.Events)
                    {
                        song.TotalEvents++;
                        chunkDurationInTicks += eventito.DeltaTime;
                        if (eventito is ChannelEvent)
                        {
                            var chanEv = (ChannelEvent)eventito;
                            if ((int)(chanEv.Channel) == 9)
                                chunkIsDrumChannel = true;
                            if (!(channelsInChunk.Contains(chanEv.Channel)))
                                channelsInChunk.Add(chanEv.Channel);
                            if (chanEv is NoteEvent)
                            {
                                var notEv = (NoteEvent)chanEv;
                                if (notEv.DeltaTime < 5)
                                    chunkNoteEventsWithNullDeltas++;
                                chunkNoteEvents++;
                                if (!pitches.Contains(notEv.NoteNumber))
                                    pitches.Add(notEv.NoteNumber);
                                if (!uniquePitches.Contains(notEv.NoteNumber.valor % 12))
                                    uniquePitches.Add(notEv.NoteNumber.valor % 12);
                                if (notEv.NoteNumber.valor > highestPitch && notEv.Channel.valor!=9)
                                    highestPitch = notEv.NoteNumber.valor;
                                if (notEv.NoteNumber.valor < lowestPitch && notEv.Channel.valor != 9)
                                    lowestPitch = notEv.NoteNumber.valor;
                                if (notEv.Channel.valor == 9)
                                {
                                    if (!percussionInstruments.Contains(notEv.NoteNumber))
                                        percussionInstruments.Add(notEv.NoteNumber);
                                }
                            }
                            if (chanEv is PitchBendEvent)
                                song.TotalPitchBendEvents++;
                            if (chanEv is ProgramChangeEvent)
                            {
                                song.TotalProgramChangeEvents++;
                                var progChEv = (ProgramChangeEvent)chanEv;
                                if (!instruments.Contains(progChEv.ProgramNumber))
                                    instruments.Add(progChEv.ProgramNumber);
                                if (hasProgramChangeEvent)
                                    song.HasMoreThanOneInstrumentPerChunk = true;
                                hasProgramChangeEvent = true;
                            }
                            if (chanEv is ControlChangeEvent)
                            {
                                song.TotalControlChangeEvents++;
                                ControlChangeEvent ctrlChEv = chanEv as ControlChangeEvent;
                                if (ctrlChEv.ControlNumber == 64)
                                    song.TotalSustainPedalEvents++;
                            }
                        }
                        else
                        {
                            song.TotalChannelIndependentEvents++;
                            if (eventito is SetTempoEvent)
                            {
                                var tempChEv = (SetTempoEvent)eventito;
                                tempoChanges.Add((int)tempChEv.MicrosecondsPerQuarterNote);
                            }
                        }
                    }
                    if (channelsInChunk.Count > 1)
                        song.HasMoreThanOneChannelPerChunk = true;
                    foreach (var ch in channelsInChunk)
                    {
                        if (!channels.Contains(ch))
                            channels.Add(ch);
                    }
                    song.TotalNoteEvents += chunkNoteEvents;
                    if (chunkIsDrumChannel)
                        song.HasPercusion = true;
                    else
                    {
                        if (chunkNoteEvents > 0 && (chunkNoteEventsWithNullDeltas / chunkNoteEvents) > 0.5)
                            song.TotalChordChunks++;
                        else
                            song.TotalMelodicChunks++;
                    }
                    if (chunkDurationInTicks > songDurationInTicks)
                        songDurationInTicks = chunkDurationInTicks;
                    if (!hasProgramChangeEvent && !instruments.Contains((SevenBitNumber)0))
                        instruments.Add((SevenBitNumber)0);
                }
                song.NumberOfTicks = (int)songDurationInTicks;
                song.TotalChannels = channels.Count;
                song.TotalInstruments = instruments.Count;
                song.TotalPercussionInstruments = percussionInstruments.Count;
                song.TotalDifferentPitches = pitches.Count;
                song.TotalUniquePitches = uniquePitches.Count;
                song.HighestPitch = highestPitch;
                song.LowestPitch = lowestPitch;
                song.TotalTempoChanges = tempoChanges.Count;
                if (tempoChanges.Count > 0)
                {
                    song.TempoInMicrosecondsPerBeat = (int)Math.Floor(tempoChanges.Average());
                    double microsoftIsShit = 120 * ((double)500000 / (double)song.TempoInMicrosecondsPerBeat);
                    song.TempoInBeatsPerMinute = (int)Math.Floor(microsoftIsShit);
                }
            }
            catch (Exception ex)
            {

            }
            return song;
        }

    }
}
