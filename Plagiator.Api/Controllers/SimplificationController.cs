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
                var songs = await Repository.GetSongs(currentPage, 3, null, 24);
                if (songs == null) break;
                foreach (var song in songs)
                {
                    var simpl = await Repository.GetSongSimplification(song.Id, 0);

                    var newSimpl = new SongSimplification()
                    {
                        Notes = simpl.Notes.Select(n => n.Clone()).ToList(),
                        SimplificationVersion = simpl.SimplificationVersion + 1,
                        SongId = simpl.SongId
                    };
                    SimplificationUtilities.RemoveBendings(newSimpl.Notes);
                    await Repository.AddSongSimplification(newSimpl);
                }
            }

            return null;
        }
    }
}
