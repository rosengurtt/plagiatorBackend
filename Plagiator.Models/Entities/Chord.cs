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
        #region constructors
        public Chord() { }

        public Chord(string pitchesAsString)
        {
            ConstructorAux(pitchesAsString);
        }
        public Chord(List<Note> notes)
        {
            var pitchesAsString = string
                .Join(",", notes.OrderBy(n => n.Pitch).Select(m => m.Pitch));
            ConstructorAux(pitchesAsString);
        }

        private void ConstructorAux(string pitchesAsString)
        {
            var re = new Regex(@"^[0-9]+(,[0-9]+)+$");
            if (!re.IsMatch(pitchesAsString))
                throw new Exception("Invalid string for a chord");
            PitchesAsString = pitchesAsString;
            var pitches = pitchesAsString.Split(",")
                    .ToList().Select(p => byte.Parse(p)).ToList();
            PitchLettersAsString = string
                .Join(",", pitches.Select(n => GetLetterPitch(n)).OrderBy(x => x));
            var intervals = new List<byte>();
            for (var i = 0; i < Pitches.Count - 1; i++)
            {
                for (var j = i+1; j < Pitches.Count; j++)
                {
                    var interval = (byte)((Pitches[j] - Pitches[i]) % 12);
                    if (!intervals.Contains(interval)) intervals.Add(interval);
                }
            }
            IntervalsAsString = string
                .Join(",", intervals.OrderBy(i => i));
        }
        #endregion
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
                     .ToList().Select(p => byte.Parse(p))
                     .ToList().OrderBy(q => q).ToList();
            }
        }
        public string IntervalsAsString { get; set; }

        public List<byte> Intervals
        {
            get
            {
                return IntervalsAsString.Split(",")
                  .ToList().Select(p => byte.Parse(p))
                  .ToList().OrderBy(q => q).ToList();
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
