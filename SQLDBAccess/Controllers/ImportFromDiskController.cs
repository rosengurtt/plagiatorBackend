using Melanchall.DryWetMidi.Core;
using Microsoft.AspNetCore.Mvc;
using Plagiator.Mucic.Utilities;
using Plagiator.Music;
using Serilog;
using SQLDBAccess.DataAccess;
using SQLDBAccess.ErrorHandling;
using SQLDBAccess.Helpers;
using SQLDBAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportFromDiskController : ControllerBase
    {
        private readonly PlagiatorContext Context;
        public ImportFromDiskController(PlagiatorContext context)
        {
            Context = context;
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
                    Style style = Context.Style.Where(s => s.Name == styleName).FirstOrDefault();
                    if (style == null)
                    {
                        style = new Style() { Name = styleName };
                        Context.Style.Add(style);
                        await Context.SaveChangesAsync();
                    }
                    var bandsPaths = Directory.GetDirectories(stylePath);
                    foreach (var bandPath in bandsPaths)
                    {
                        var bandName = FileSystemUtils.GetLastDirectoryName(bandPath);
                        Band band = Context.Band.Where(b => b.Name == bandName).FirstOrDefault();
                        if (band == null)
                        {
                            band = new Band()
                            {
                                Name = bandName,
                                Style = style
                            };
                            Context.Band.Add(band);
                            await Context.SaveChangesAsync();
                        }
                        var songsPaths = Directory.GetFiles(bandPath);
                        foreach (var songPath in songsPaths)
                        {
                            var lelo = MidiFile.Read(songPath, null);
                            var chunkitos = lelo.Chunks;

                            var originalMidiBase64encoded = FileSystemUtils.GetBase64encodedFile(songPath);
                            string normalizedMidiBase64encoded = "";
                            try
                            {
                                normalizedMidiBase64encoded = NormalizedSong.GetSongAsBase64EncodedMidi(originalMidiBase64encoded);
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex, $"Failes to normalize song {Path.GetFileName(songPath)}");
                            }
                            Song song = new Song()
                            {
                                Name = Path.GetFileName(songPath),
                                Band = band,
                                Style = style,
                                OriginalMidiBase64Encoded = originalMidiBase64encoded,
                                 NormalizedMidiBase64Encoded= normalizedMidiBase64encoded
                            };
                            try
                            {
                                Context.Song.Add(song);
                                await Context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {

                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {

            }
                return Ok(new ApiOKResponse("All files processed"));
   
        }
    }
}
