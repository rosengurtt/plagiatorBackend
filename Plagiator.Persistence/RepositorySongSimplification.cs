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
        public async Task<SongSimplification> GetSongSimplification(Song song, int version)
        {
            return await Context.SongSimplifications
                .Where(s => s.SongId == song.Id && s.SimplificationVersion == version)
                .Include("Notes.PitchBending")
                .FirstOrDefaultAsync();
        }

        public async Task UpdateSongSimplification(SongSimplification simpl)
        {
            Context.SongSimplifications.Update(simpl);
            await Context.SaveChangesAsync();
        }

        public async Task<SongSimplification> AddSongSimplification(SongSimplification simpl)
        {
            Context.SongSimplifications.Add(simpl);
            await Context.SaveChangesAsync();
            return simpl;
        }

    }
}
