using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Mucic.Utilities
{
    public class MidiProcessing
    {
        public static List<MidiEvent> ExtractChannelIndependentEvents(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            List<MidiEvent> retObj = new List<MidiEvent>();

            foreach (TrackChunk chunk in midiFile.Chunks.ToList())
            {
                long acumulatedTimeInTicks = 0;
                foreach (var eventito in chunk.Events)
                {
                    acumulatedTimeInTicks += eventito.DeltaTime;

                    if (!(eventito is ChannelEvent))
                    {
                        var e = eventito.Clone();
                        e.DeltaTime = acumulatedTimeInTicks;
                        retObj.Add(e);
                    }
                }
            }
            return ConvertAccumulatedTimeToDeltaTime(retObj);
        }

        public static List<MidiEvent> GetEventsOfType(TrackChunk chunk, MidiEventType type)
        {
            List<MidiEvent> retObj = new List<MidiEvent>();
            long acumulatedTimeInTicks = 0;
            foreach (var eventito in chunk.Events)
            {
                acumulatedTimeInTicks += eventito.DeltaTime;

                if (eventito.EventType == type)
                {
                    var e = eventito.Clone();
                    e.DeltaTime = acumulatedTimeInTicks;
                    retObj.Add(e);
                }
            }
            return ConvertAccumulatedTimeToDeltaTime(retObj);
        }

        public static List<MidiEvent> GetEventsOfType(string base64encodedMidiFile, MidiEventType type)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            List<MidiEvent> retObj = new List<MidiEvent>();

            foreach (TrackChunk chunk in midiFile.Chunks.ToList())
            {
                long acumulatedTimeInTicks = 0;
                foreach (var eventito in chunk.Events)
                {
                    acumulatedTimeInTicks += eventito.DeltaTime;

                    if (eventito.EventType == type)
                    {
                        var e = eventito.Clone();
                        e.DeltaTime = acumulatedTimeInTicks;
                        retObj.Add(e);
                    }
                }
            }
            return ConvertAccumulatedTimeToDeltaTime(retObj);
        }

        public static List<MidiEvent> MergeEvents(List<MidiEvent> list1, List<MidiEvent> list2)
        {
            if (list1.Count == 0) return list2;
            if (list2.Count == 0) return list1;

            var retObj = new List<MidiEvent>();
            list1 = ConvertDeltaTimeToAccumulatedTime(list1);
            list2 = ConvertDeltaTimeToAccumulatedTime(list2);
            int i1 = 0;
            int i2 = 0;
            while (i1 < list1.Count && i2 < list2.Count)
            {
                if ((i2 == list2.Count && i1 < list1.Count) || list1[i1].DeltaTime <= list2[i2].DeltaTime)
                    retObj.Add(list1[i1++]);
                else retObj.Add(list2[i2++]);
            }
            if (i1 < list1.Count)
                for (var i = i1; i < list1.Count; i++) retObj.Add(list1[i]);
            if (i2 < list2.Count)
                for (var i = i2; i < list2.Count; i++) retObj.Add(list2[i]);
            return ConvertAccumulatedTimeToDeltaTime(retObj);
        }

        public static List<MidiEvent> ConvertDeltaTimeToAccumulatedTime(List<MidiEvent> list)
        {
            var returnObj = new List<MidiEvent>();
            long accumulatedTime = 0;
            foreach(var e in list)
            {
                var clonito = e.Clone();
                accumulatedTime += e.DeltaTime;
                clonito.DeltaTime = accumulatedTime;
                returnObj.Add(clonito);
            }
            return returnObj;
        }
        public static List<MidiEvent> ConvertAccumulatedTimeToDeltaTime(List<MidiEvent> list)
        {
            var returnObj = new List<MidiEvent>();
            foreach (var e in list) returnObj.Add(e.Clone());
            for (int i = list.Count - 1; i > 0; i--)
                returnObj[i].DeltaTime -= returnObj[i - 1].DeltaTime;
            return returnObj;
        }


        public static List<Bar> GetAllMusicalEventsOfSong(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            List<Bar> retObj = new List<Bar>();

            


            return retObj;
        }
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
            TimeSignatureEvent currentTimeSignature = (TimeSignatureEvent)timeSignatureEvents[0];
            SetTempoEvent currentTempo = (SetTempoEvent)setTempoEvents[0];        
            int timeSigIndex = 0;
            int tempoIndex = 0;
            long currentTick = 0;
            while (currentTick < songDurationInTicks)
            {
                long timeOfNextTimeSignatureEvent = songDurationInTicks;
                if (timeSignatureEvents.Count - 1 > timeSigIndex)
                    timeOfNextTimeSignatureEvent = timeSignatureEvents[timeSigIndex + 1].DeltaTime;
                long timeOfNextSetTempoEvent = songDurationInTicks;
                if (setTempoEvents.Count - 1 > tempoIndex)
                    timeOfNextSetTempoEvent = setTempoEvents[tempoIndex + 1].DeltaTime;

                long lastTickOfBarToBeAdded = currentTimeSignature.Numerator * ticksPerBeat + currentTick;

                while ((lastTickOfBarToBeAdded < timeOfNextTimeSignatureEvent && lastTickOfBarToBeAdded < timeOfNextSetTempoEvent) ||
                    (lastTickOfBarToBeAdded > songDurationInTicks))
                {
                    var bar = new Bar()
                    {
                        TimeSignature = new TimeSignature
                        {
                            numerator = currentTimeSignature.Numerator,
                            denominator = currentTimeSignature.Denominator
                        },
                        Tempo = currentTempo.MicrosecondsPerQuarterNote
                    };
                    retObj.Add(bar);
                    currentTick += currentTimeSignature.Numerator * ticksPerBeat;
                    lastTickOfBarToBeAdded += currentTimeSignature.Numerator * ticksPerBeat;
                    if (currentTick >= songDurationInTicks)
                        break;
                }
                if (currentTick >= timeOfNextTimeSignatureEvent)
                    timeSigIndex++;
                if (lastTickOfBarToBeAdded > timeOfNextSetTempoEvent)
                    tempoIndex++;
            }
            return retObj;
        }

        public static long GetSongDurationInTicks(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            long duration = 0;
            foreach(TrackChunk chunk in midiFile.Chunks)
            {
                long trackDuration = 0;
                foreach(var eventito in chunk.Events)
                {
                    trackDuration += eventito.DeltaTime;
                }
                if (trackDuration > duration) duration = trackDuration;
            }
            return duration;
        }

        public static int GetTicksPerBeatOfSong(string base64encodedMidiFile)
        {
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            var ticksPerQuarter = midiFile.TimeDivision as TicksPerQuarterNoteTimeDivision;
            if (ticksPerQuarter != null) return ticksPerQuarter.TicksPerQuarterNote;
            else throw new Exception("The midi file doesn't have a value for TicksPerBeat");
        }


        public static List<Note> GetNotesOfSong(string base64encodedMidi)
        {
            var retObj = new List<Note>();
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            long songDuration = GetSongDurationInTicks(base64encodedMidi);

            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                long currentTick = 0;
                var eventCount = 0;
                var currentNotes = new List<Note>();
                var currentIntrument = GeneralMidi2Program.AcousticGrandPiano;
                var programChangeEvents = GetEventsOfType(chunk, MidiEventType.ProgramChange);
                if (programChangeEvents.Count > 0)
                    currentIntrument = GetInstrument(programChangeEvents, 0);

                while (currentTick< songDuration && eventCount< chunk.Events.Count) {
                    var eventito = chunk.Events[eventCount];
                    if (eventito is NoteOnEvent)
                    {
                        NoteOnEvent noteOnEvent = eventito as NoteOnEvent;
                        ProcessNoteOn(noteOnEvent, currentNotes, retObj, currentTick, currentIntrument);
                    }
                    if (eventito is NoteOffEvent)
                    {
                        NoteOffEvent noteOffEvent = eventito as NoteOffEvent;
                        var noteOnEvent = new NoteOnEvent
                        {
                            DeltaTime = eventito.DeltaTime,
                            Velocity = new SevenBitNumber(0),
                            NoteNumber = noteOffEvent.NoteNumber
                        };
                        ProcessNoteOn(noteOnEvent, currentNotes, retObj, currentTick, currentIntrument);
                    }
                    if (eventito is ControlChangeEvent)
                    {

                    }

                }
            }
        }
        private static void ProcessNoteOn(NoteOnEvent noteOnEvent, List<Note> currentNotes,
            List<Note> retObj, long currentTick, GeneralMidi2Program currentIntrument)
        {
            
            if (noteOnEvent.NoteNumber > 0)
            {
                var notita = new Note
                {
                    Instrument = currentIntrument,
                    Pitch = noteOnEvent.NoteNumber,
                    StartSinceBeginningOSongInTicks = noteOnEvent.DeltaTime + currentTick
                };
                currentNotes.Add(notita);
            }
            else
            {
                var notota = currentNotes.Where(n => n.Pitch == noteOnEvent.NoteNumber).FirstOrDefault();
                if (notota != null)
                {
                    notota.EndSinceBeginnintOfSongInTicks = currentTick + noteOnEvent.DeltaTime;
                    retObj.Add(notota);
                    currentNotes.Remove(notota);
                }
            }
        }
        private static GeneralMidi2Program GetInstrument(List<MidiEvent> events,int index)
        {
            var eventito = events[index];
            var instrumentCode = ((ProgramChangeEvent)(eventito)).ProgramNumber;
            return (GeneralMidi2Program)(instrumentCode.valor);
        }
        public static string GetNormalizedVersionOfMidiFileBase64encoded(string base64encodedMidiFile)
        {
            var soret = GetEmptyBarsOfSong(base64encodedMidiFile);
            return base64encodedMidiFile;
        }
    }
}
