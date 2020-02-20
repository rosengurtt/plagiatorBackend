using Microsoft.EntityFrameworkCore;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.DataAccess
{
    public partial class SongRepository : ISongRepository
    {
        public async Task<MelodyPattern> GetMelodyPatternById(int melodyPatternId)
        {
            return await Context.MelodyPatterns.FindAsync(melodyPatternId);
        }
        public async Task<MelodyPattern> GetMelodyPatternByPitchPatternIdAndRythmPatternId(
            int pitchPatternId,
            int rythmPatternId)
        {
            return await Context.MelodyPatterns
                .Where(a => a.PitchPatternId== pitchPatternId &
                a.RythmPatternId== rythmPatternId).FirstOrDefaultAsync();
        }
        public async Task<MelodyPattern> AddMelodyPattern(MelodyPattern melodyPattern)
        {
            Context.MelodyPatterns.Add(melodyPattern);
            await Context.SaveChangesAsync();
            return melodyPattern;
        }

        public async Task<MelodyPatternOccurrence> GetMelodyPatternOccurrenceById(int arpOcId)
        {
            return await Context.MelodyPatternOccurrences.FindAsync(arpOcId);
        }
        public async Task<MelodyPatternOccurrence> GetMelodyPatternOccurrencesForSongVersionIdAndMelodyPatternId(
            int songVersionId,
            int melodyPatternId)
        {
            return await Context.MelodyPatternOccurrences
                .Where(a => a.SongVersionId== songVersionId &
                a.MelodyPatternId==melodyPatternId).FirstOrDefaultAsync();
        }
        public async Task<MelodyPatternOccurrence> AddMelodyPatternOccurrence(MelodyPatternOccurrence arpOc)
        {
            Context.MelodyPatternOccurrences.Add(arpOc);
            await Context.SaveChangesAsync();
            return arpOc;
        }
    }
}
