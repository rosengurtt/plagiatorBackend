﻿using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public async Task<Song> GetSongByIdAsync(long songId)
        {
            return await Context.Songs.Include(x => x.Style)
                .Include(x => x.Band).Include(y=>y.SongSimplifications)
                .Include(z=>z.SongStats)
                .FirstOrDefaultAsync(x => x.Id == songId);
        }
        public async Task<Song> GetSongByNameAndBandAsync(string songName, string bandName)
        {
            return await Context.Songs.FirstOrDefaultAsync(x => x.Name == songName & x.Band.Name == bandName);
        }

        public async Task<List<Song>> GetSongsAsync(int pageNo = 1,
            int pageSize = 1000,
            string startWith = null,
            long? bandId = null)
        {
            if (bandId != null && bandId > 0)
            {
                return await Context.Songs
                    .Where(x => x.BandId == bandId).Skip((pageNo - 1) * pageSize)
                    .Take(pageSize).ToListAsync();
            }
            if (string.IsNullOrEmpty(startWith))
                return await Context.Songs.Skip((pageNo - 1) * pageSize)
                    .Take(pageSize).ToListAsync();
            else
                return await Context.Songs.Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

        }

        
        public async Task<int> GetNumberOfSongsAsync(
            string startWith = null,
            long? bandId = null)
        {
            if (bandId != null && bandId > 0)
            {
                return await Context.Songs
                    .Where(x => x.BandId == bandId).CountAsync();
            }
            if (string.IsNullOrEmpty(startWith))
                return await Context.Songs.CountAsync();
            else
                return await Context.Songs.Where(x => x.Name.StartsWith(startWith)).CountAsync();
        }


        public async Task<Song> UpdateSongAsync(Song song)
        {
            Context.Entry(await Context.Songs.FirstOrDefaultAsync(x => x.Id == song.Id))
                .CurrentValues.SetValues(song);
            await Context.SaveChangesAsync();
            return song;
        }
        public async Task<Song> AddSongAsync(Song song)
        {
            Context.Songs.Add(song);
            await Context.SaveChangesAsync();
            return song;
        }


        public async Task DeleteSongAsync(long songId)
        {
            var songItem = await Context.Songs.Include(x => x.Style)
                .FirstOrDefaultAsync(x => x.Id == songId);
            if (songItem == null)
                throw new ApplicationException($"No song with id {songId}");

            Context.Songs.Remove(songItem);
            await Context.SaveChangesAsync();
        }

    }
}
