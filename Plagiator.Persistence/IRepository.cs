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
        Task<List<Song>> GetSongs(int page, int pageSize, string startWith, int? bandId);
        Task<int> GetNumberOfSongs(int page, int pageSize, string startWith, int? bandId);
        Task<Song> GetSongById(int songId);
        Task<Song> GetSongByNameAndBand(string songName, string bandName);
        Task<Song> UpdateSong(Song song);
        Task<Song> AddSong(Song song);
        Task DeleteSong(int songId);

        // Style
        Task<List<Style>> GetStyles(int pageNo, int pageSize, string startWith);
        Task<int> GetNumberOfStyles(int pageNo, int pageSize, string startWith);
        Task<Style> GetStyleById(int styleId);
        Task<Style> GetStyleByName(string name);
        Task<Style> AddStyle(Style style);
        Task<Style> UpdateStyle(Style style);
        Task DeleteStyle(int styleId);

        // Bands
        Task<List<Band>> GetBands(int pageNo, int pageSize, string startWith, int? styleId);
        Task<int> GetNumberOfBands(int pageNo, int pageSize, string startWith, int? styleId);
        Task<Band> GetBandById(int bandId);
        Task<Band> GetBandByName(string name);
        Task<Band> AddBand(Band band);
        Task<Band> UpdateBand(Band band);
        Task DeleteBand(int bandId);

        // Time Signatures

        Task<TimeSignature> GetTimeSignature(TimeSignature timeSignature);

        // Patterns
        Task<Pattern> GetPatternByIdAsync(int patternId);
        Task<Pattern> GetPatternByStringAndTypeAsync(string patternString, PatternType patternType);
        Pattern AddPattern(Pattern pattern);
        Task<Occurrence> GetOccurrenceByIdAsync(int occurrenceId);
        Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(int songSimplificationId, int patternId);
        Task<Occurrence> AddOccurrence(Occurrence occurrence);

        // SongSimplifications
        Task<SongSimplification> AddSongSimplification(SongSimplification simpl);
        Task UpdateSongSimplification(SongSimplification simpl);
        Task<SongSimplification> GetSongSimplification(Song song, int version);

        // Chords
        Chord AddChord(Chord chord);
        Task<Chord> GetChordByIdAsync(int chordId);
        Task<Chord> GetChordByStringAsync(string pitchesAsString);

        // Melodies
        Task<Melody> AddMelodyAsync(Melody melody);
    }
}
