using Melanchall.DryWetMidi.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Plagiator.Analysis;
using Plagiator.Analysis.Chords;
using Plagiator.Analysis.Patterns;
using Plagiator.Api.ErrorHandling;
using Plagiator.Api.Helpers;
using Plagiator.Midi;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using Plagiator.Persistence;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private IRepository Repository;
        public AnalysisController(IRepository Repository)
        {
            this.Repository = Repository;
        }

        [HttpGet]
        public async Task<IActionResult> ProcesarTodas()
        {
            int songsProcessed = 0;
            int page = 0;
            int totalSongs = await Repository.GetNumberOfSongsAsync();
            while (songsProcessed < totalSongs) {
                page++;
                var songs = await Repository.GetSongsAsync(page, 10);
                foreach (var s in songs)
                {
                    var song = await Repository.GetSongByIdAsync(s.Id);
                    if (SongHasAlreadyBeenProcessed(song)) continue;
                    song.SongSimplifications[0] = await Repository.GetSongSimplificationAsync(song.Id, 0);
                    ProcesameLosPatterns(song);
                    await ProcesameLosAcordes(song);
                }
                songsProcessed += songs.Count();
            }         

            return Ok("Todo bien papi");
        }
        private bool SongHasAlreadyBeenProcessed(Song song)
        {
            return Repository.AreOccurrencesForSongSimplificationAlreadyProcessed(song.SongSimplifications[0].Id);
        }

        // GET: api/Song/5
        [HttpGet("{songId}")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await Repository.GetSongByIdAsync(songId);

            if (song == null)
                return NotFound(new ApiResponse(404));

            song.SongSimplifications[0] = await Repository.GetSongSimplificationAsync(songId, 0);

            ProcesameLosPatterns(song);
            await ProcesameLosAcordes(song);
            return Ok("Todo bien papi");
        }
        private async Task<Song> ProcesameLosAcordes(Song song)
        {
            try
            {        
                var chordsOccur = ChordsUtilities.GetChordsOfSimplification(song.SongSimplifications[0]);
                if (chordsOccur.Keys.Count > 0)
                {
                    song.SongSimplifications[0].Chords = new List<Chord>();
                    song.SongSimplifications[0].ChordOccurrences = new List<ChordOccurrence>();
                }
                foreach (var chordAsString in chordsOccur.Keys)
                {
                    var chordito = await Repository.GetChordByStringAsync(chordAsString);
                    if (chordito == null)
                    {
                        chordito = new Chord(chordAsString);
                        chordito = Repository.AddChord(chordito);
                    }
                    song.SongSimplifications[0].Chords.Add(chordito);
                    foreach (var oc in chordsOccur[chordAsString])
                    {
                        oc.ChordId = chordito.Id;
                        oc.Id = Repository.AddChordOccurence(oc).Id;
                        song.SongSimplifications[0].ChordOccurrences.Add(oc);
                    }
                }

                //var melodies = SimplificationUtilities.GetMelodiesOfSimplification(song.SongSimplifications[0]);
                //foreach (var melody in melodies)
                //{
                //    await Repository.AddMelodyAsync(melody);
                //}

                return song;
            }
            catch (Exception sorete)
            {
                return null;
            }
        }

        private List<Occurrence> ProcesameLosPatterns(Song song)
        {
            if (!Repository.AreOccurrencesForSongSimplificationAlreadyProcessed(song.SongSimplifications[0].Id)) {
                var allOccurrences = PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Pitch).Values
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Rythm).Values)
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Melody).Values)
                .ToList().SelectMany(x => x).ToList();

                var patitos = allOccurrences.Select(x => x.Pattern).Distinct().ToList();

                foreach (var pat in patitos)
                {
                    try
                    {
                        var patito =  Repository.GetPatternByStringAndType(pat.AsString, pat.PatternTypeId);

                        if (patito == null)
                        {
                            patito = Repository.AddPattern(pat);
                        }
                        foreach (var oc in allOccurrences.Where(x => x.Pattern.AsString == patito.AsString))
                        {
                            oc.Pattern = patito;
                            oc.PatternId = patito.Id;
                            oc.SongSimplificationId = song.SongSimplifications[0].Id;
                            Repository.AddOccurrence(oc);
                        }
                    }
                    catch(Exception sacamela)
                    {

                    }
                }
            }
            return song.SongSimplifications[0].Occurrences;
        }

    }
}
