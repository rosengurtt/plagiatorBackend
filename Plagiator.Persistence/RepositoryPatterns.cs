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
    public partial class Repository : IRepository
    {
        public async Task<Pattern> GetPatternByIdAsync(int patternId)
        {
            return await Context.Patterns.FindAsync(patternId);
        }


        public async Task<Pattern> GetPatternByStringAndTypeAsync(string patternString, PatternType patternType)
        {
            return await Context.Patterns
                .Where(a => a.AsString == patternString & a.PatternTypeId == patternType).FirstOrDefaultAsync();
        }
        public Pattern AddPattern(Pattern pattern)
        {
            return Context.Patterns
                  .FromSqlRaw($"insert into Patterns(AsString, PatternTypeId) values ('{pattern.AsString}', {(int)pattern.PatternTypeId}) SELECT * FROM Patterns WHERE id = SCOPE_IDENTITY();")
                  .ToList().FirstOrDefault();
        }

        public void DetachSong(Song song)
        {
            Context.Entry(song).State = EntityState.Detached;
        }
        public async Task<Occurrence> GetOccurrenceByIdAsync(int ocId)
        {
            return await Context.Occurrences.FindAsync(ocId);
        }

        public async Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(
            int SongSimplificationId, int patternId)
        {
            return await Context.Occurrences.Join(
                Context.SongSimplifications,
                occurrence => occurrence.SongSimplificationId,
                songVersion => songVersion.Id,
                (occurrence, songSimplification) => new Occurrence
                {
                    Id = occurrence.Id,
                    SongSimplificationId = songSimplification.Id,
                    PatternId = occurrence.PatternId,
                    FirstNoteId = occurrence.FirstNoteId,
                    FirstNote = occurrence.FirstNote,
                    Pattern = occurrence.Pattern
                }
                )
                .Where(a => a.SongSimplificationId == SongSimplificationId & a.PatternId == patternId).ToListAsync();
        }

        public async Task<Occurrence> AddOccurrence(Occurrence oc)
        {
            Context.Occurrences.Add(oc);
            await Context.SaveChangesAsync();
            return oc;

        }
    }
}
