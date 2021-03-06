﻿using Microsoft.EntityFrameworkCore;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.DataAccess
{
    public partial class SongRepository : ISongRepository
    {

        public async Task<Pattern> GetPatternByIdAsync(int patternId)
        {
            return await Context.Patterns.FindAsync(patternId);
        }

       
        public async Task<Pattern> GetPatternByStringAndTypeAsync(string patternString, PatternType patternType)
        {
            return await Context.Patterns
                .Where(a => a.AsString == patternString & a.PatternTypeId== patternType).FirstOrDefaultAsync();
        }
        public Pattern AddPatternAsync(Pattern pattern)
        {
            return Context.Patterns
                  .FromSqlRaw($"insert into Pattern(AsString, PatternTypeId) values ('{pattern.AsString}', {(int)pattern.PatternTypeId}) SELECT * FROM Pattern WHERE id = SCOPE_IDENTITY();")
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

        public async Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(int songVersionId, int patternId)
        {
            return await Context.Occurrences.Join(
                Context.SongVersions,
                occurrence => occurrence.SongVersionId,
                songVersion => songVersion.Id,
                (occurrence, songVersion) => new Occurrence
                {
                    Id = occurrence.Id,
                    SongVersionId = songVersion.Id,
                    PatternId = occurrence.PatternId,
                    FirstNoteId = occurrence.FirstNoteId,
                    FirstNote = occurrence.FirstNote,
                    Pattern = occurrence.Pattern
                }
                )
                .Where(a => a.SongVersionId == songVersionId & a.PatternId == patternId).ToListAsync();
        }

        public async Task<Occurrence> AddOccurrence(Occurrence oc)
        {
            Context.Occurrences.Add(oc);
            await Context.SaveChangesAsync();
            return oc;

        }
    
    }
}
