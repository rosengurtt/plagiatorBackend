using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public Chord AddChord(Chord chord)
        {
            return Context.Chords
                  .FromSqlRaw(@$"insert into Chords(PitchesAsString, PitchLettersAsString, IntervalsAsString) 
                                values ('{chord.PitchesAsString}', '{chord.PitchLettersAsString}', '{chord.IntervalsAsString}') 
                        SELECT * FROM Chords WHERE Id = SCOPE_IDENTITY();")
                  .ToList().FirstOrDefault();
        }

        public async Task<Chord> GetChordByIdAsync(int chordId)
        {
            return await Context.Chords.FindAsync(chordId);
        }


        public async Task<Chord> GetChordByStringAsync(string pitchesAsString)
        {
            return await Context.Chords
                .Where(a => a.PitchesAsString == pitchesAsString).FirstOrDefaultAsync();
        }
    }
}
