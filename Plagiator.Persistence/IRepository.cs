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
        Task<List<Song>> GetSongsAsync(int page, int pageSize, string startWith = null, long? bandId = null);
        Task<int> GetNumberOfSongsAsync(string startWith = null, long? bandId = null);
        Task<Song> GetSongByIdAsync(long songId);
        Task<Song> GetSongByNameAndBandAsync(string songName, string bandName);
        Task<Song> UpdateSongAsync(Song song);
        Task<Song> AddSongAsync(Song song);
        Task DeleteSongAsync(long songId);

        // Style
        Task<List<Style>> GetStylesAsync(int pageNo, int pageSize, string startWith);
        Task<int> GetNumberOfStylesAsync(int pageNo, int pageSize, string startWith);
        Task<Style> GetStyleByIdAsync(long styleId);
        Task<Style> GetStyleByNameAsync(string name);
        Task<Style> AddStyleAsync(Style style);
        Task<Style> UpdateStyleAsync(Style style);
        Task DeleteStyleAsync(long styleId);

        // Bands
        Task<List<Band>> GetBandsAsync(int pageNo, int pageSize, string startWith = null, long? styleId = null);
        Task<int> GetNumberOfBandsAsync(string startWith = null, long? styleId = null);
        Task<Band> GetBandByIdAsync(long bandId);
        Task<Band> GetBandByNameAsync(string name);
        Task<Band> AddBandAsync(Band band);
        Task<Band> UpdateBandAsync(Band band);
        Task DeleteBandAsync(long bandId);

        // Time Signatures

        Task<TimeSignature> GetTimeSignatureAsync(TimeSignature timeSignature);

        // Patterns
        Task<Pattern> GetPatternByIdAsync(long patternId);
        Pattern GetPatternByStringAndType(string patternString, PatternType patternType);
        Pattern AddPattern(Pattern pattern);
        Task<Occurrence> GetOccurrenceByIdAsync(long occurrenceId);
        Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternIdAsync(long songSimplificationId, long patternId);
        Occurrence AddOccurrence(Occurrence oc);
        Task<List<Occurrence>> GetPatternOccurrencesOfSongSimplificationAsync(long songSimplificationId);
        bool AreOccurrencesForSongSimplificationAlreadyProcessed(long songSimplificationId);

        // SongSimplifications
        Task<SongSimplification> AddSongSimplificationAsync(SongSimplification simpl);
        Task UpdateSongSimplificationAsync(SongSimplification simpl);
        Task<SongSimplification> GetSongSimplificationAsync(long songId, int version);
        Task<List<Note>> GetSongSimplificationNotesAsync(long songSimplificationId);

        // Chords
        Chord AddChord(Chord chord);
        Task<Chord> GetChordByIdAsync(int chordId);
        Task<Chord> GetChordByStringAsync(string pitchesAsString);
        ChordOccurrence AddChordOccurence(ChordOccurrence chordoc);

        // Melodies
        Task<Melody> AddMelodyAsync(Melody melody);
    }
}
