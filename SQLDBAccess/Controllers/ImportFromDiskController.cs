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
            int count = 0;
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
                            if (!songPath.ToLower().EndsWith(".mid")) continue;
                            try
                            {
                                var lelo = MidiFile.Read(songPath, null);
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex, $"Song {songPath} esta podrida");
                                continue;
                            }

                            var originalMidiBase64encoded = FileSystemUtils.GetBase64encodedFile(songPath);
                            //string normalizedMidiBase64encoded = "";
                            //try
                            //{
                            //    normalizedMidiBase64encoded = NormalizedSong.GetSongAsBase64EncodedMidi(originalMidiBase64encoded);
                            //}
                            //catch (Exception ex)
                            //{
                            //    Log.Error(ex, $"Failed to normalize song {Path.GetFileName(songPath)}");
                            //}
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
                           
                            await SongRepository.AddSong(song);
                                
                         
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Exception raised when running ImportMidis");
            }
            return Ok(new ApiOKResponse("All files processed"));

        }

        [HttpGet]
        [Route("analize")]
        public async Task<ActionResult> ProcessSong(string songPath)
        {
            if (!songPath.ToLower().EndsWith(".mid")) return BadRequest(new ApiBadRequestResponse("not midi"));
            try
            {
                var lelo = MidiFile.Read(songPath, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Song {songPath} esta podrida");
                BadRequest(new ApiBadRequestResponse("Shit file"));
            }

            var originalMidiBase64encoded = FileSystemUtils.GetBase64encodedFile(songPath);

            Song song = new Song()
            {
                Name = Path.GetFileName(songPath),
                OriginalMidiBase64Encoded = originalMidiBase64encoded
            };
            var soret = MidiProcessing.GetEventsOfChannel(originalMidiBase64encoded, 9);
            song = MidiProcessing.ComputeSongStats(song);
            song.TimeSignature = await SongRepository.GetTimeSignature(song.TimeSignature);



            return Ok(new ApiOKResponse("Gracias papi"));
        }
    }
}
