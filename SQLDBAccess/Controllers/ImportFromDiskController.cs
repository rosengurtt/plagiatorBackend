using Melanchall.DryWetMidi.Core;
using Microsoft.AspNetCore.Mvc;
using Plagiator.Music.SongUtilities;
using Plagiator.Music;
using Serilog;
using SQLDBAccess.DataAccess;
using SQLDBAccess.ErrorHandling;
using SQLDBAccess.Helpers;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SQLDBAccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportFromDiskController : ControllerBase
    {
        private ISongRepository SongRepository;
        public ImportFromDiskController(PlagiatorContext context, ISongRepository SongRepository)
        {
            this.SongRepository = SongRepository;
        }

        [HttpGet]
        public async Task<ActionResult> ImportMidis(string musicFolderPath = "C:\\music\\midi")
        {
            try
            {
                var styles = Directory.GetDirectories(musicFolderPath);
                foreach (var stylePath in styles)
                {
                    var styleName = FileSystemUtils.GetLastDirectoryName(stylePath);
                    Style style = await SongRepository.GetStyleByName(styleName);
                    if (style == null)
                    {
                        style = new Style() { Name = styleName };
                        await SongRepository.AddStyle(style);
                    }
                    var bandsPaths = Directory.GetDirectories(stylePath);
                    foreach (var bandPath in bandsPaths)
                    {
                        var bandName = FileSystemUtils.GetLastDirectoryName(bandPath);
                        Band band = await SongRepository.GetBandByName(bandName);
                        if (band == null)
                        {
                            band = new Band()
                            {
                                Name = bandName,
                                Style = style
                            };
                            await SongRepository.AddBand(band);
                        }
                        var songsPaths = Directory.GetFiles(bandPath);
                        foreach (var songPath in songsPaths)
                        {
                            var songita = await SongRepository.GetSongByNameAndBand(Path.GetFileName(songPath), bandName);
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

            var stylito = await SongRepository.GetStyleByName(style);
            var bandita = await SongRepository.GetBandByName(band);
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

                var originalMidiBase64encoded = FileSystemUtils.GetBase64encodedFile(songPath);
                originalMidiBase64encoded = MidiProcessing.NormalizeTicksPerQuarterNote(originalMidiBase64encoded);

                Song song = new Song()
                {
                    Name = Path.GetFileName(songPath),
                    Band = band,
                    Style = style,
                    OriginalMidiBase64Encoded = originalMidiBase64encoded,
                };
                song = MidiProcessing.ComputeSongStats(song);
                song.TimeSignature = await SongRepository.GetTimeSignature(song.TimeSignature);
                song.NormalizeSong();

                foreach (var oc in song.Versions[0].Occurrences)
                {
                    var pattern = oc.Pattern;
                    var patito = await SongRepository.GetPatternByStringAndTypeAsync(pattern.AsString, pattern.PatternTypeId);
                    if (patito == null)
                    {
                        patito = await SongRepository.AddPatternAsync(pattern);
                    }
                    oc.Pattern = patito;
                    oc.PatternId = patito.Id;
                }

                await SongRepository.AddSong(song);


                var outputPath = Path.Combine(@"C:\music\procesados", song.Name);
                var bytes = Convert.FromBase64String(song.ProcessedMidiBase64Encoded);
                System.IO.File.WriteAllBytes(outputPath, bytes);
            }
            catch(Exception sorete)
            {

            }
        }
    }
}
