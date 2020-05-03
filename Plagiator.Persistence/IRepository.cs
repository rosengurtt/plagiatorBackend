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
        Task<List<Song>> GetSongs(int page, int pageSize, string startWith, long? bandId);
        Task<int> GetNumberOfSongs(int page, int pageSize, string startWith, long? bandId);
        Task<Song> GetSongById(long songId);
        Task<Song> GetSongByNameAndBand(string songName, string bandName);
        Task<Song> UpdateSong(Song song);
        Task<Song> AddSong(Song song);
        Task DeleteSong(long songId);

        // Style
        Task<List<Style>> GetStyles(int pageNo, int pageSize, string startWith);
        Task<int> GetNumberOfStyles(int pageNo, int pageSize, string startWith);
        Task<Style> GetStyleById(long styleId);
        Task<Style> GetStyleByName(string name);
        Task<Style> AddStyle(Style style);
        Task<Style> UpdateStyle(Style style);
        Task DeleteStyle(long styleId);

        // Bands
        Task<List<Band>> GetBands(int pageNo, int pageSize, string startWith, long? styleId);
        Task<int> GetNumberOfBands(int pageNo, int pageSize, string startWith, long? styleId);
        Task<Band> GetBandById(long bandId);
        Task<Band> GetBandByName(string name);
        Task<Band> AddBand(Band band);
        Task<Band> UpdateBand(Band band);
        Task DeleteBand(long bandId);

        // Time Signatures

        Task<TimeSignature> GetTimeSignature(TimeSignature timeSignature);

        // Patterns
        Task<Pattern> GetPatternByIdAsync(long patternId);
        Pattern GetPatternByStringAndType(string patternString, PatternType patternType);
        Pattern AddPattern(Pattern pattern);
        Task<Occurrence> GetOccurrenceByIdAsync(long occurrenceId);
        Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(long songSimplificationId, long patternId);
        Occurrence AddOccurrence(Occurrence oc);
        Task<List<Occurrence>> GetPatternOccurrencesOfSongSimplification(long songSimplificationId);
        bool AreOccurrencesForSongSimplificationAlreadyProcessed(long songSimplificationId);

        // SongSimplifications
        Task<SongSimplification> AddSongSimplification(SongSimplification simpl);
        Task UpdateSongSimplification(SongSimplification simpl);
        Task<SongSimplification> GetSongSimplification(long songId, int version);
        Task<List<Note>> GetSongSimplificationNotes(long songSimplificationId);

        // Chords
        Chord AddChord(Chord chord);
        Task<Chord> GetChordByIdAsync(int chordId);
        Task<Chord> GetChordByStringAsync(string pitchesAsString);

        // Melodies
        Task<Melody> AddMelodyAsync(Melody melody);
    }
}
