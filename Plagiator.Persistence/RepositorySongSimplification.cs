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
            var songSimpl = await Context.SongSimplifications
                .Where(s => s.SongId == songId && s.SimplificationVersion == version)
                .FirstOrDefaultAsync();
            try
            {

                songSimpl.Notes = await (from ss in Context.SongSimplifications
                                         join ssn in Context.SongSimplificationNotes on ss.Id equals ssn.SongSimplificationId
                                         join n in Context.Notes on ssn.NoteId equals n.Id
                                         where ss.SongId == songId && ss.SimplificationVersion == version
                                         select n).ToListAsync();
            }
            catch(Exception dfdsfas)
            {

            }
            return songSimpl;
        }

        public async Task UpdateSongSimplificationAsync(SongSimplification simpl)
        {
            Context.SongSimplifications.Update(simpl);
            await Context.SaveChangesAsync();
        }

        public async Task<SongSimplification> AddSongSimplificationAsync(SongSimplification simpl)
        {
            try
            {
                Context.SongSimplifications.Add(simpl);
                await Context.SaveChangesAsync();
                var newNotes = simpl.Notes.Where(n => n.Id == 0).ToList();
                foreach (var n in newNotes)
                {
                    Context.Notes.Add(n);
                }
                await Context.SaveChangesAsync();
                foreach (var n in simpl.Notes)
                {
                    Context.SongSimplificationNotes.Add(new SongSimplificationNote
                    {
                        NoteId = n.Id,
                        SongSimplificationId = simpl.Id
                    });
                }
                await Context.SaveChangesAsync();
            }
            catch(Exception fdsafasdfa)
            {

            }
            return simpl;

        }

        public async Task<List<Note>> GetSongSimplificationNotesAsync(long songSimplificationId)
        {
            return await (from ss in Context.SongSimplifications
                          join ssn in Context.SongSimplificationNotes on ss.Id equals ssn.SongSimplificationId
                          join n in Context.Notes on ssn.NoteId equals n.Id
                          where ss.Id == songSimplificationId
                          select n).ToListAsync();
        }
    }
}
