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
    public class SimplificationController : ControllerBase
    {
        private IRepository Repository;
        public SimplificationController(IRepository Repository)
        {
            this.Repository = Repository;
        }
        [HttpGet]
        public async Task<ActionResult> SimplifySongs()
        {
            var currentPage = 0;
            while (true)
            {
                currentPage++;
                var songs = await Repository.GetSongs(currentPage, 3, null, null);
                if (songs == null) break;
                foreach (var song in songs)
                {
                    var simpl = await Repository.GetSongSimplification(song, 0);
                    var chordsOccur = SimplificationUtilities.GetChordsOfSimplification(simpl);
                    if (chordsOccur.Keys.Count > 0)
                    {
                        simpl.Chords = new List<Chord>();
                        simpl.ChordOccurrences = new List<ChordOccurrence>();
                    }
                    foreach (var chordAsString in chordsOccur.Keys)
                    {
                        var chordito = await Repository.GetChordByStringAsync(chordAsString);
                        if (chordito == null)
                        {
                            chordito = new Chord(chordAsString);
                            chordito = Repository.AddChord(chordito);
                        }
                        simpl.Chords.Add(chordito);
                        foreach (var oc in chordsOccur[chordAsString])
                        {
                            oc.ChordId = chordito.Id;
                            simpl.ChordOccurrences.Add(oc);
                        }
                    }
                    await Repository.UpdateSongSimplification(simpl);
                }
            }

            return null;
        }
    }
}
