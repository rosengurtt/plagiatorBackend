using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Standards;
using Plagiator.Music.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plagiator.Music.SongUtilities
{
    public partial class MidiProcessing
    {
        public static List<Note> GetNotesOfSong(string base64encodedMidiFile, bool percussionNotes=false)
        {
            var retObj = new List<Note>();
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            long songDuration = GetSongDurationInTicks(base64encodedMidiFile);
            var isSustainPedalOn = false;
            var notesOnBecauseOfSustainPedal = new List<Note>();


            var cleanedChunks = SeparateChannelsIntoDifferentChunks(midiFile.Chunks);
            if (cleanedChunks.Count > 16)
            {
                Log.Error("Creation of CleanedChunks produced more than 16 chunks");
                throw new Exception("Creation of CleanedChunks produced more than 16 chunks");
            }
            foreach (TrackChunk chunk in cleanedChunks)
            {
                long currentTick = 0;
                var eventCount = 0;
                var currentNotes = new List<Note>();
                var currentIntrument = GeneralMidi2Program.AcousticGrandPiano;
                var programChangeEvents = GetEventsOfType(chunk, MidiEventType.ProgramChange);
                if (programChangeEvents.Count > 0)
                    currentIntrument = GetInstrument(programChangeEvents, 0);

                while (currentTick < songDuration && eventCount < chunk.Events.Count)
                {
                    var eventito = chunk.Events[eventCount];
                    currentTick += eventito.DeltaTime;

                    if (!percussionNotes && IsPercussionEvent(eventito)) continue;
                    if (percussionNotes && !IsPercussionEvent(eventito)) continue;

                    if (IsSustainPedalEventOn(eventito))
                        isSustainPedalOn = true;

                    if (IsSustainPedalEventOff(eventito))
                    {
                        isSustainPedalOn = false;
                        foreach(var n in notesOnBecauseOfSustainPedal)
                        {
                            ProcessNoteOff(n.Pitch, currentNotes, retObj, currentTick, currentIntrument);
                        }
                    }
                    if (eventito is NoteOnEvent)
                    {
                        NoteOnEvent noteOnEvent = eventito as NoteOnEvent;
                        if (noteOnEvent.Velocity > 0 || isSustainPedalOn == false)
                        {
                            ProcessNoteOn(noteOnEvent.NoteNumber, noteOnEvent.Velocity,
                                currentNotes, retObj, currentTick, currentIntrument);
                        }
                    }
                    if (eventito is NoteOffEvent && isSustainPedalOn == false)
                    {
                        NoteOffEvent noteOffEvent = eventito as NoteOffEvent;
                        ProcessNoteOff(noteOffEvent.NoteNumber, currentNotes, retObj, currentTick, currentIntrument);
                    }
                    if (eventito is PitchBendEvent)
                    {
                        PitchBendEvent bendito = eventito as PitchBendEvent;
                        foreach (var notita in currentNotes)
                        {
                            PitchBendEvent maldito = bendito.Clone() as PitchBendEvent;
                            maldito.DeltaTime = currentTick;
                            notita.PitchBendingEvents.Add(maldito);
                        }
                    }
                    if (eventito is ControlChangeEvent)
                    {
                        ControlChangeEvent chEv = eventito as ControlChangeEvent;
                        foreach (var notita in currentNotes)
                        {
                            ControlChangeEvent changito = chEv.Clone() as ControlChangeEvent;
                            changito.DeltaTime = currentTick;
                            notita.ControlChangeEvents.Add(changito);
                        }
                    }
                    eventCount++;
                }
            }
            return retObj;
        }

        private static bool IsSustainPedalEventOn(MidiEvent eventito)
        {
            if (!(eventito is ControlChangeEvent)) return false;
            ControlChangeEvent eve = eventito as ControlChangeEvent;
            if (eve.ControlNumber == 64 && eve.ControlValue>63) return true;
            return false;
        }
        private static bool IsSustainPedalEventOff(MidiEvent eventito)
        {
            if (!(eventito is ControlChangeEvent)) return false;
            ControlChangeEvent eve = eventito as ControlChangeEvent;
            if (eve.ControlNumber == 64 && eve.ControlValue < 64) return true;
            return false;
        }


        private static void ProcessNoteOn(byte pitch, byte volume, List<Note> currentNotes,
                List<Note> retObj, long currentTick, GeneralMidi2Program currentIntrument)
        {

            if (volume > 0)
            {
                var notita = new Note
                {
                    Instrument = currentIntrument,
                    Pitch = pitch,
                    StartSinceBeginningOSongInTicks = currentTick,
                    Volume = volume
                };
                currentNotes.Add(notita);
            }
            else
            {
                var notota = currentNotes.Where(n => n.Pitch == pitch).FirstOrDefault();
                if (notota != null)
                {
                    notota.EndSinceBeginnintOfSongInTicks = currentTick;
                    retObj.Add(notota);
                    currentNotes.Remove(notota);
                }
            }
        }
        private static void ProcessNoteOff(byte pitch, List<Note> currentNotes,
         List<Note> retObj, long currentTick, GeneralMidi2Program currentIntrument)
        {
            ProcessNoteOn(pitch, 0, currentNotes, retObj, currentTick, currentIntrument);
        }

        private static bool IsPercussionEvent(MidiEvent eventito)
        {
            if (!(eventito is ChannelEvent)) return false;
            var chEv = eventito as ChannelEvent;
            if (chEv.Channel == 9) return true;
            return false;
        }


    }
}
