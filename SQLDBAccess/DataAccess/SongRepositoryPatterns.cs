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

        public async Task<PitchPattern> GetPitchPatternById(int pitchPatternId)
        {
            return await Context.PitchPatterns.FindAsync(pitchPatternId);
        }

        public async Task<RythmPattern> GetRythmPatternById(int rythmPatternId)
        {
            return await Context.RythmPatterns.FindAsync(rythmPatternId);
        }
        public async Task<PitchPattern> GetPitchPatternByPatternString(string pitchPatternString)
        {
            return await Context.PitchPatterns
                .Where(a => a.AsString == pitchPatternString).FirstOrDefaultAsync();
        }
        public async Task<PitchPattern> AddPitchPattern(PitchPattern pitchPattern)
        {
            Context.PitchPatterns.Add(pitchPattern);
            await Context.SaveChangesAsync();
            return pitchPattern;
        }
        public async Task<RythmPattern> GetRythmPatternByPatternString(string rythmPatternString)
        {
            return await Context.RythmPatterns
                .Where(a => a.AsString == rythmPatternString).FirstOrDefaultAsync();
        }
        public async Task<RythmPattern> AddRythnPattern(RythmPattern rythnPattern)
        {
            Context.RythmPatterns.Add(rythnPattern);
            await Context.SaveChangesAsync();
            return rythnPattern;
        }
    }
}
