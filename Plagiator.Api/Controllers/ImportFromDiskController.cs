
using Melanchall.DryWetMidi.Core;
using Microsoft.AspNetCore.Mvc;
using Plagiator.Api.ErrorHandling;
using Plagiator.Api.Helpers;
using Plagiator.Midi;
using Plagiator.Models.Entities;
using Plagiator.Persistence;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Api.Controllers
{
    public class ImportFromDiskController : ControllerBase
    {
        private IRepository Repository;
        public ImportFromDiskController(PlagiatorContext context, IRepository Repository)
        {
            this.Repository = Repository;
        }
        [HttpGet]
        public async Task<ActionResult> ImportMidis(string musicFolderPath = @"C:\music\midi")
        {
            try
            {
                var styles = Directory.GetDirectories(musicFolderPath);
                foreach (var stylePath in styles)
                {
                    var styleName = FileSystemUtils.GetLastDirectoryName(stylePath);
                    Style style = await Repository.GetStyleByName(styleName);
                    if (style == null)
                    {
                        style = new Style() { Name = styleName };
                        await Repository.AddStyle(style);
                    }
                    var bandsPaths = Directory.GetDirectories(stylePath);
                    foreach (var bandPath in bandsPaths)
                    {
                        var bandName = FileSystemUtils.GetLastDirectoryName(bandPath);
                        Band band = await Repository.GetBandByName(bandName);
                        if (band == null)
                        {
                            band = new Band()
                            {
                                Name = bandName,
                                Style = style
                            };
                            await Repository.AddBand(band);
                        }
                        var songsPaths = Directory.GetFiles(bandPath);
                        foreach (var songPath in songsPaths)
                        {
                            var songita = await Repository.GetSongByNameAndBand(Path.GetFileName(songPath), bandName);
                            if (songita == null) await ProcesameLaSong(songPath, band, style);
                        }
                    }
                }
            }
            catch (Exception soreton)
            {
                Log.Error(soreton, $"Exception raised when running ImportMidis");
                return new StatusCodeResult(500);
            }
            return Ok(new ApiOKResponse("All files processed"));
        }
        [HttpGet]
        [Route("analize")]
        public async Task<ActionResult> ProcessSong(string songPath, string band, string style)
        {
            if (string.IsNullOrEmpty(songPath)) return BadRequest(new ApiBadRequestResponse("Parameter songPath missing"));
            if (string.IsNullOrEmpty(band)) return BadRequest(new ApiBadRequestResponse("Parameter band missing"));
            if (string.IsNullOrEmpty(style)) return BadRequest(new ApiBadRequestResponse("Parameter style missing"));

            var stylito = await Repository.GetStyleByName(style);
            var bandita = await Repository.GetBandByName(band);
            await ProcesameLaSong(songPath, bandita, stylito);

            return Ok(new ApiOKResponse("Gracias papi"));
        }

        private async Task ProcesameLaSong(string songPath, Band band, Style style)
        {
            try
            {
                if (!songPath.ToLower().EndsWith(".mid")) return;
                try
                {
                    var lelo = MidiFile.Read(songPath, null);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Song {songPath} esta podrida");
                    return;
                }

                var midiBase64encoded = FileSystemUtils.GetBase64encodedFile(songPath);
                midiBase64encoded = MidiUtilities.NormalizeTicksPerQuarterNote(midiBase64encoded);

                Song song = new Song()
                {
                    Name = Path.GetFileName(songPath),
                    Band = band,
                    Style = style,
                    MidiBase64Encoded = midiBase64encoded,
                };
                song.SongStats = MidiUtilities.GetSongStats(midiBase64encoded);
                song.TimeSignature = await Repository.GetTimeSignature(song.TimeSignature);


                song = await Repository.AddSong(song);


            }            }
            catch (Exception sorete)
            {

            }

        }
}
