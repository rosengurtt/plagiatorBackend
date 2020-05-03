using Melanchall.DryWetMidi.Core;
using Microsoft.AspNetCore.Mvc;
using Plagiator.Analysis;
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

        // GET: api/Song/5
        [HttpGet("{songId}")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await Repository.GetSongById(songId);

            if (song == null)
                return NotFound(new ApiResponse(404));

            song.SongSimplifications[0] = await Repository.GetSongSimplification(songId, 0);

            var occurrences = ProcesameLosPatterns(song);
            return Ok(new ApiOKResponse(occurrences));
        }
        private async Task<Song> ProcesameLaSong(long SongId)
        {
            try
            {
                var song = await Repository.GetSongById(SongId);


        
                var chordsOccur = SimplificationUtilities.GetChordsOfSimplification(song.SongSimplifications[0]);
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
                        song.SongSimplifications[0].ChordOccurrences.Add(oc);
                    }
                }
                var melodies = SimplificationUtilities.GetMelodiesOfSimplification(song.SongSimplifications[0]);
                foreach (var melody in melodies)
                {
                    await Repository.AddMelodyAsync(melody);
                }

                return song;
            }
            catch (Exception sorete)
            {
                return null;
            }
        }

        private async Task<List<Occurrence>> ProcesameLosPatterns(Song song)
        {
            var occurs =  Repository.GetPatternOccurrencesOfSongSimplification(song.SongSimplifications[0].Id);
            if (occurs == null || occurs.Count == 0) {
                var allOccurrences = PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Pitch).Values
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Rythm).Values)
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Melody).Values)
                .ToList().SelectMany(x => x).ToList();

                var patitos = allOccurrences.Select(x => x.Pattern).Distinct().ToList();

                foreach (var pat in patitos)
                {
                    try
                    {
                        var patito = await Repository.GetPatternByStringAndTypeAsync(pat.AsString, pat.PatternTypeId);

                        if (patito == null)
                        {
                            patito = Repository.AddPattern(pat);
                        }
                        foreach (var oc in allOccurrences.Where(x => x.Pattern.AsString == patito.AsString))
                        {
                            oc.Pattern = patito;
                            oc.PatternId = patito.Id;
                            oc.SongSimplificationId = song.SongSimplifications[0].Id;
                        }
                    }
                    catch(Exception e)
                    {

                    }
                }
                song.SongSimplifications[0].Occurrences = allOccurrences;
                await Repository.UpdateSong(song);
            }
            return song.SongSimplifications[0].Occurrences;
        }

    }
}
