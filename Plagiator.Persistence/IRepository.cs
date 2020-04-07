using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    public interface IRepository
    {
        // Songs
        public Task<List<Song>> GetSongs(int page, int pageSize, string startWith, int? bandId);
        public Task<int> GetNumberOfSongs(int page, int pageSize, string startWith, int? bandId);
        public Task<Song> GetSongById(int songId);
        public Task<Song> GetSongByNameAndBand(string songName, string bandName);
        public Task<Song> UpdateSong(Song song);
        public Task<Song> AddSong(Song song);
        public Task DeleteSong(int songId);

        // Style
        public Task<List<Style>> GetStyles(int pageNo, int pageSize, string startWith);
        public Task<int> GetNumberOfStyles(int pageNo, int pageSize, string startWith);
        public Task<Style> GetStyleById(int styleId);
        public Task<Style> GetStyleByName(string name);
        public Task<Style> AddStyle(Style style);
        public Task<Style> UpdateStyle(Style style);
        public Task DeleteStyle(int styleId);

        // Bands
        public Task<List<Band>> GetBands(int pageNo, int pageSize, string startWith, int? styleId);
        public Task<int> GetNumberOfBands(int pageNo, int pageSize, string startWith, int? styleId);
        public Task<Band> GetBandById(int bandId);
        public Task<Band> GetBandByName(string name);
        public Task<Band> AddBand(Band band);
        public Task<Band> UpdateBand(Band band);
        public Task DeleteBand(int bandId);

        // Time Signatures

        public Task<TimeSignature> GetTimeSignature(TimeSignature timeSignature);

        // Patterns
        Task<Pattern> GetPatternByIdAsync(int patternId);
        Task<Pattern> GetPatternByStringAndTypeAsync(string patternString, PatternType patternType);
        Pattern AddPattern(Pattern pattern);
        Task<Occurrence> GetOccurrenceByIdAsync(int occurrenceId);
        Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(int songSimplificationId, int patternId);
        Task<Occurrence> AddOccurrence(Occurrence occurrence);

    }
}
