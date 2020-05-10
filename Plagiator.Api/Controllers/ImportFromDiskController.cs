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
    public class ImportFromDiskController : ControllerBase
    {
        private IRepository Repository;
        public ImportFromDiskController(IRepository Repository)
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
                    Style style = await Repository.GetStyleByNameAsync(styleName);
                    if (style == null)
                    {
                        style = new Style() { Name = styleName };
                        await Repository.AddStyleAsync(style);
                    }
                    var bandsPaths = Directory.GetDirectories(stylePath);
                    foreach (var bandPath in bandsPaths)
                    {
                        var bandName = FileSystemUtils.GetLastDirectoryName(bandPath);
                        Band band = await Repository.GetBandByNameAsync(bandName);
                        if (band == null)
                        {
                            band = new Band()
                            {
                                Name = bandName,
                                Style = style
                            };
                            await Repository.AddBandAsync(band);
                        }
                        var songsPaths = Directory.GetFiles(bandPath);
                        foreach (var songPath in songsPaths)
                        {
                            var songita = await Repository.GetSongByNameAndBandAsync(Path.GetFileName(songPath), bandName);
                            if (songita == null)
                            {
                                var song = await ProcesameLaSong(songPath, band, style);
                                //if (song != null) await ProcesameLosPatterns(song);
                            }
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

            var stylito = await Repository.GetStyleByNameAsync(style);
            var bandita = await Repository.GetBandByNameAsync(band);
            var song = await ProcesameLaSong(songPath, bandita, stylito);

            return Ok(new ApiOKResponse("Gracias papi"));
        }

        private async Task<Song> ProcesameLaSong(string songPath, Band band, Style style)
        {
            try
            {
                if (!songPath.ToLower().EndsWith(".mid")) return null;
                try
                {
                    var lelo = MidiFile.Read(songPath, null);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Song {songPath} esta podrida");
                    return null;
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
                // The following line is  used to get the id of the time signature. If we don't
                // provide the id when saving the song, it will create a duplicated time signature
                // in the TimeSignatures table
                song.SongStats.TimeSignature = await Repository.GetTimeSignatureAsync(song.SongStats.TimeSignature);

                song.SongSimplifications = new List<SongSimplification>();
                song.SongSimplifications.Add(MidiUtilities.GetSimplificationZeroOfSong(midiBase64encoded));
                song.Bars = MidiUtilities.GetBarsOfSong(midiBase64encoded, song.SongSimplifications[0]);
                song.SongSimplifications[0].Notes = MidiUtilities.QuantizeNotes(song.SongSimplifications[0], song.Bars)
                    .ToList();
                song.TempoChanges = MidiUtilities.GetTempoChanges(midiBase64encoded);
                song.SongStats.NumberBars = song.Bars.Count();
                song = await Repository.AddSongAsync(song);              

                return song;
            }
            catch (Exception sorete)
            {
                return null;
            }
        }
  
        private async Task ProcesameLosPatterns(Song song)
        {
            var allOccurrences =  PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Pitch).Values
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Rythm).Values)
                .Concat(PatternUtilities.FindPatternsOfTypeInSong(song, 0, PatternType.Melody).Values)
                .ToList().SelectMany(x=>x).ToList();

            foreach (var oc in allOccurrences)
            {
                var pattern = oc.Pattern;
                var patito =  Repository.GetPatternByStringAndType(pattern.AsString, pattern.PatternTypeId);
                if (patito == null)
                {
                    patito = Repository.AddPattern(pattern);
                }
                oc.Pattern = patito;
                oc.PatternId = patito.Id;
                oc.SongSimplificationId = song.SongSimplifications[0].Id;
            }
            song.SongSimplifications[0].Occurrences = allOccurrences;
            await Repository.UpdateSongAsync(song);
        }
    }
}
