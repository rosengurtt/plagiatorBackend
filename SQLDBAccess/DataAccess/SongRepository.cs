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
        private readonly PlagiatorContext Context;
        public SongRepository(PlagiatorContext context)
        {
            Context = context;
        }

        public async Task< Song> GetSongById(int songId)
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
    public interface ISongRepository
    {
        public Task<List<Song>> GetSongs(int page, int pageSize, string startWith, int? bandId);
        public Task<int> GetNumberOfSongs(int page, int pageSize, string startWith, int? bandId);
        public Task<Song> GetSongById(int songId);
        public Task<Song> GetSongByNameAndBand(string songName, string bandName);
        public Task<Song> UpdateSong(Song song);
        public Task<Song> AddSong(Song song);
        public Task DeleteSong(int songId);
        public Task<List<Style>> GetStyles(int pageNo, int pageSize, string startWith);
        public Task<int> GetNumberOfStyles(int pageNo, int pageSize, string startWith);
        public Task<Style> GetStyleById(int styleId);
        public Task<Style> GetStyleByName(string name);
        public Task<Style> AddStyle(Style style);
        public Task<Style> UpdateStyle(Style style);
        public Task DeleteStyle(int styleId);
        public Task<List<Band>> GetBands(int pageNo, int pageSize, string startWith, int? styleId);
        public Task<int> GetNumberOfBands(int pageNo, int pageSize, string startWith, int? styleId);

        public Task<Band> GetBandById(int bandId);
        public Task<Band> GetBandByName(string name);
        public Task<Band> AddBand(Band band);
        public Task<Band> UpdateBand(Band band);
        public Task DeleteBand(int bandId);

        public Task<TimeSignature> GetTimeSignature(TimeSignature ts);

        Task<Pattern> GetPatternByIdAsync(int patternId);


        Task<Pattern> GetPatternByStringAndTypeAsync(string patternString, PatternType patternType);
        Pattern AddPatternAsync(Pattern pattern);
        Task<Occurrence> GetOccurrenceByIdAsync(int ocId);

        Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(int songVersionId, int patternId);

        Task<Occurrence> AddOccurrence(Occurrence oc);

        void DetachSong(Song song);
    }
}
