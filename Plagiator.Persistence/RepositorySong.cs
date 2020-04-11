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
        public async Task<Song> GetSongById(int songId)
        {
            return await Context.Songs.Include(x => x.Style)
                .Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == songId);
        }
        public async Task<Song> GetSongByNameAndBand(string songName, string bandName)
        {
            return await Context.Songs.FirstOrDefaultAsync(x => x.Name == songName & x.Band.Name == bandName);
        }

        public async Task<List<Song>> GetSongs(int pageNo = 1,
            int pageSize = 1000,
            string startWith = null,
            int? bandId = null)
        {
            if (bandId != null)
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

        
        public async Task<int> GetNumberOfSongs(
            int pageNo = 1,
            int pageSize = 1000,
            string startWith = null,
            int? bandId = null)
        {
            if (bandId != null)
            {
                return await Context.Songs
                    .Where(x => x.BandId == bandId).Skip((pageNo - 1) * pageSize)
                    .Take(pageSize).CountAsync();
            }
            if (string.IsNullOrEmpty(startWith))
                return await Context.Songs.Skip((pageNo - 1) * pageSize)
                    .Take(pageSize).CountAsync();
            else
                return await Context.Songs.Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
        }


        public async Task<Song> UpdateSong(Song song)
        {
            Context.Entry(await Context.Songs.FirstOrDefaultAsync(x => x.Id == song.Id))
                .CurrentValues.SetValues(song);
            await Context.SaveChangesAsync();
            return song;
        }
        public async Task<Song> AddSong(Song song)
        {
            Context.Songs.Add(song);
            await Context.SaveChangesAsync();
            return song;
        }


        public async Task DeleteSong(int songId)
        {
            var songItem = await Context.Songs.Include(x => x.Style)
                .FirstOrDefaultAsync(x => x.Id == songId);
            if (songItem == null)
                throw new ApplicationException($"No song with id {songId}");

            Context.Songs.Remove(songItem);
            await Context.SaveChangesAsync();
        }

        public async Task<List<Style>> GetStyles(
            int pageNo = 1,
            int pageSize = 10,
            string startWith = null)
        {
            if (string.IsNullOrEmpty(startWith))
                return await Context.Styles.OrderBy(x => x.Name).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Styles.OrderBy(x => x.Name).Where(x => x.Name.StartsWith(startWith)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<int> GetNumberOfStyles(
            int pageNo = 1,
            int pageSize = 10,
            string startWith = null)
        {
            if (string.IsNullOrEmpty(startWith))
                return await Context.Styles.OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
            else
                return await Context.Styles.OrderBy(x => x.Name)
                    .Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
        }
    }
}
