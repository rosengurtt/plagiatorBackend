using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public async Task<Melody> AddMelodyAsync(Melody melody)
        {
            Context.Melodies.Add(melody);
            await Context.SaveChangesAsync();
            foreach (var note in melody.Notes)
            {
                Context.MelodyNotes.Add(new MelodyNote()
                {
                    MelodyId = melody.Id,
                    NoteId = note.Id
                });
            }
            await Context.SaveChangesAsync();
            return melody;
        }
    }
}
