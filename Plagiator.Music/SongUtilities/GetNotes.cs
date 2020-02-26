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
        public static List<Note> GetNotesOfSong(string base64encodedMidiFile)
        {
            var retObj = new List<Note>();
            var midiFile = MidiFile.Read(base64encodedMidiFile);
            long songDuration = GetSongDurationInTicks(base64encodedMidiFile);
            var isSustainPedalOn = false;
            var notesOnBecauseOfSustainPedal = new List<Note>();
            var instrumentOfChannel = new GeneralMidi2Program[16];

            int chunkNo = -1;
            foreach (TrackChunk chunk in midiFile.Chunks)
            {
                chunkNo++;
                var currentNotes = new List<Note>();
                long currentTick = 0;

                foreach (MidiEvent eventito in chunk.Events)
                {
                    currentTick += eventito.DeltaTime;

                    if (eventito is ProgramChangeEvent)
                    {
                        var pg = eventito as ProgramChangeEvent;
                        instrumentOfChannel[pg.Channel] = (GeneralMidi2Program)pg.ProgramNumber.valor;
                        continue;
                    }

                    if (IsSustainPedalEventOn(eventito))
                    {
                        isSustainPedalOn = true;
                        continue;
                    }

                    if (IsSustainPedalEventOff(eventito))
                    {
                        isSustainPedalOn = false;
                        foreach(var n in notesOnBecauseOfSustainPedal)
                        {
                            ProcessNoteOff(n.Pitch, currentNotes, retObj, currentTick, 
                                n.Instrument, chunkNo);
                        }
                        continue;
                    }
                    if (eventito is NoteOnEvent)
                    {
                        NoteOnEvent noteOnEvent = eventito as NoteOnEvent;
                        if (noteOnEvent.Velocity > 0 || isSustainPedalOn == false)
                        {
                            ProcessNoteOn(noteOnEvent.NoteNumber, noteOnEvent.Velocity,
                                currentNotes, retObj, currentTick, 
                                instrumentOfChannel[noteOnEvent.Channel],
                                IsPercussionEvent(eventito), chunkNo);
                        }
                        continue;
                    }
                    if (eventito is NoteOffEvent && isSustainPedalOn == false)
                    {
                        NoteOffEvent noteOffEvent = eventito as NoteOffEvent;
                        ProcessNoteOff(noteOffEvent.NoteNumber, currentNotes, retObj, currentTick,
                            instrumentOfChannel[noteOffEvent.Channel], chunkNo);
                        continue;
                    }
                    if (eventito is PitchBendEvent)
                    {
                        PitchBendEvent bendito = eventito as PitchBendEvent;
                        foreach (var notita in currentNotes)
                        {
                            PitchBendEvent maldito = bendito.Clone() as PitchBendEvent;
                            maldito.DeltaTime = currentTick;
                            notita.PitchBending.Add(new PitchBendItem
                            {
                                Note = notita,
                                Pitch = maldito.PitchValue,
                                TicksSinceBeginningOfSong = maldito.DeltaTime
                            });
                        }
                        continue;
                    }
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
                List<Note> retObj, long currentTick, GeneralMidi2Program instrument,
                bool isPercussion, int voice)
        {

            if (volume > 0)
            {
                var notita = new Note
                {
                    Instrument = instrument,
                    Pitch = pitch,
                    StartSinceBeginningOfSongInTicks = currentTick,
                    Volume = volume,
                    IsPercussion = isPercussion,
                    Voice = voice
                };
                currentNotes.Add(notita);
            }
            else
            {
                var notota = currentNotes.Where(n => n.Pitch == pitch).FirstOrDefault();
                if (notota != null)
                {
                    notota.EndSinceBeginningOfSongInTicks = currentTick;
                    retObj.Add(notota);
                    currentNotes.Remove(notota);
                }
            }
        }
        private static void ProcessNoteOff(byte pitch, List<Note> currentNotes,
         List<Note> retObj, long currentTick, GeneralMidi2Program intrument, int voice)
        {
            ProcessNoteOn(pitch, 0, currentNotes, retObj, currentTick, intrument, false, voice);
        }

        private static bool IsPercussionEvent(MidiEvent eventito)
        {
            if (!(eventito is ChannelEvent)) return false;
            var chEv = eventito as ChannelEvent;
            if (chEv.Channel == 9) 
                return true;
            return false;
        }


    }
}
