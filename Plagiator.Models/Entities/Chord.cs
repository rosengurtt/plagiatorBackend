using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Plagiator.Models.Entities
{
    /// <summary>
    /// Represents a group of notes played at the same time
    /// It consists of notes that belong to a SongSimplification
    /// The chord is associated to a song simplification
    /// </summary>
    public class Chord
    {
        public Chord() { }

        public Chord(string pitchesAsString)
        {
            var re = new Regex(@"^[0-9]+(,[0-9]+)+$");
            if (!re.IsMatch(pitchesAsString))
                throw new Exception("Invalid string for a chord");
            PitchesAsString = pitchesAsString;
            var pitches = pitchesAsString.Split(",")
                    .ToList().Select(p => byte.Parse(p)).ToList();
            PitchLettersAsString = string
                .Join(",", pitches.Select(n => GetLetterPitch(n)).OrderBy(x => x));
        }
        public Chord(List<Note> notes)
        {
            PitchesAsString = string
                .Join(",", notes.OrderBy(n => n.Pitch).Select(m => m.Pitch));
            PitchLettersAsString = string
                .Join(",", notes.Select(n => GetLetterPitch(n.Pitch)).OrderBy(x => x));
        }

        public long Id { get; set; }
        
        /// <summary>
        /// The pitches here are numbers from 0 to 127
        /// </summary>
        public string PitchesAsString { get; set; }

        /// <summary>
        /// The pitches are expressed as the letters
        /// A, Ab, B, Bb, C, D, Db, E, Eb, F, G, Gb
        /// </summary>
        public string PitchLettersAsString { get; set; }

        public List<byte> Pitches
        {
            get
            {
               return PitchesAsString.Split(",")
                    .ToList().Select(p => byte.Parse(p)).ToList();
            }
        }

        private static string GetLetterPitch(byte pitch)
        {
            pitch = (byte) (pitch % 12);
            switch (pitch)
            {
                case 0:
                    return "C";
                case 1:
                    return "Db";
                case 2:
                    return "D";
                case 3:
                    return "Eb";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "Gb";
                case 7:
                    return "G";
                case 8:
                    return "Ab";
                case 9:
                    return "A";
                case 10:
                    return "Bb";
                case 11:
                    return "B";
                default:
                    return "";
            }
        }
    }
}
