using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Threading.Tasks;
using Plagiator.Persistence;
using Plagiator.Api.ErrorHandling;
using Plagiator.Models.Entities;

namespace SQLDBAccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private IRepository Repository;

        public SongController(IRepository Repository)
        {
            this.Repository = Repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetSongs(
            int pageNo = 1,
            int pageSize = 1000,
            string startWith = null,
            int? bandId = null)
        {
            var totalSongs =await Repository.GetNumberOfSongs(pageNo, pageSize, startWith, bandId);
         
            var songs = await Repository.GetSongs(pageNo, pageSize, startWith, bandId);
            var retObj = new
            {
                page = pageNo,
                totalPages = (int)Math.Ceiling((double)totalSongs / pageSize),
                songs
            };
            return Ok(new ApiOKResponse(retObj));
        }

        // GET: api/Song/5
        [HttpGet("{songId}")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await Repository.GetSongById(songId);

            if (song == null)
                return NotFound(new ApiResponse(404));

            return Ok(new ApiOKResponse(song));
        }

        // PUT: api/Song/5
        [HttpPut("{songId}")]
        public async Task<ActionResult> PutSong(int songId, Song song)
        {
            if (song.Id != songId)
                return BadRequest(new ApiBadRequestResponse("Id passed in url does not match id passed in body."));

            try
            {
                await Repository.UpdateSong(song);
                return Ok(new ApiOKResponse(song));
            }
            catch (ApplicationException)
            {
                return NotFound(new ApiResponse(404, "No song with that id"));
            }
        }

        // POST: api/Song
        [HttpPost]
        public async Task<ActionResult> PostSong(Song song)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var addedSong = await Repository.AddSong(song);
                    return Ok(new ApiOKResponse(addedSong));
                }
                catch (DbUpdateException)
                {
                    return Conflict(new ApiResponse(409, "There is already a Song with that name."));
                }
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse(ModelState));
            }
        }

        // DELETE: api/Song/5
        [HttpDelete("{songId}")]
        public async Task<ActionResult> DeleteSong(int songId)
        {
            try
            {
                await Repository.DeleteSong(songId);
                return Ok(new ApiOKResponse("Record deleted"));
            }
            catch (ApplicationException)
            {
                return NotFound(new ApiResponse(404, "No song with that id"));
            }
        }
    }
}