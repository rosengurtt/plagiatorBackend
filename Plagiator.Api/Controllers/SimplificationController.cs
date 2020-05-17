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
                var songs = await Repository.GetSongsAsync(currentPage, 3);
                if (songs == null) break;
                foreach (var song in songs)
                {
                    var simpl = await Repository.GetSongSimplificationAsync(song.Id, 0);

                    var simpl1 = new SongSimplification()
                    {
                        Notes = simpl.Notes.Select(n => n.Clone())
                            .OrderBy(x => x.StartSinceBeginningOfSongInTicks).ToList(),
                        SimplificationVersion = 1,
                        SongId = simpl.SongId,
                        NumberOfVoices = simpl.NumberOfVoices
                    };
                    simpl1.Notes = SimplificationUtilities.RemoveBendings(simpl1.Notes);
                    simpl1.Notes = SimplificationUtilities.RemoveEmbelishments(simpl1.Notes);

                    await Repository.AddSongSimplificationAsync(simpl1);

                    var simpl2 = new SongSimplification()
                    {
                        Notes = simpl1.Notes.Select(n => n.Clone())
                            .OrderBy(x => x.StartSinceBeginningOfSongInTicks).ToList(),
                        SimplificationVersion = 2,
                        SongId = simpl.SongId,
                        NumberOfVoices = simpl.NumberOfVoices
                    };
                    simpl2.Notes = SimplificationUtilities.RemoveNonEssentialNotes(simpl2.Notes,80);

                    await Repository.AddSongSimplificationAsync(simpl2);

                    var simpl3 = new SongSimplification()
                    {
                        Notes = simpl2.Notes.Select(n => n.Clone())
                            .OrderBy(x => x.StartSinceBeginningOfSongInTicks).ToList(),
                        SimplificationVersion = 3,
                        SongId = simpl.SongId,
                        NumberOfVoices = simpl.NumberOfVoices
                    };
                    simpl3.Notes = SimplificationUtilities.RemoveNonEssentialNotes(simpl3.Notes, 80);

                    await Repository.AddSongSimplificationAsync(simpl3);

                }
            }

            return null;
        }
    }
}
