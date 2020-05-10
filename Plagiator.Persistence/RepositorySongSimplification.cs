using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public async Task<SongSimplification> GetSongSimplificationAsync(long songId, int version)
        {
            return await Context.SongSimplifications
                .Where(s => s.SongId == songId && s.SimplificationVersion == version)
                .Include("Notes.PitchBending")
                .FirstOrDefaultAsync();
        }

        public async Task UpdateSongSimplificationAsync(SongSimplification simpl)
        {
            Context.SongSimplifications.Update(simpl);
            await Context.SaveChangesAsync();
        }

        public async Task<SongSimplification> AddSongSimplificationAsync(SongSimplification simpl)
        {
            Context.SongSimplifications.Add(simpl);
            await Context.SaveChangesAsync();
            return simpl;
        }

        public async Task<List<Note>> GetSongSimplificationNotesAsync(long songSimplificationId)
        {
            return await Context.Notes
                .Where(x => x.SongSimplificationId == songSimplificationId)
                .OrderBy(y => y.StartSinceBeginningOfSongInTicks).ToListAsync();
        }
    }
}
