using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLDBAccess.DataAccess;
using SQLDBAccess.ErrorHandling;
using SQLDBAccess.Models;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly PlagiatorContext Context;

        public SongController(PlagiatorContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetSongs(
            int pageNo = 1,
            int pageSize = 1000,
            string startWith = null,
            int? bandId = null)
        {
            if (bandId != null)
            {
                return await Context.Song.Where(x => x.BandId == bandId).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            if (string.IsNullOrEmpty(startWith))
                return await Context.Song.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Song.Where(x => x.Name.StartsWith(startWith)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/Song/5
        [HttpGet("{songId}")]
        public async Task<IActionResult> GetSong(int songId)
        {
            var song = await Context.Song.Include(x => x.Style).Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == songId);

            if (song == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(new ApiOKResponse(song));
        }

        // PUT: api/Song/5
        [HttpPut("{songId}")]
        public async Task<ActionResult> PutSong(int songId, Song song)
        {
            if (song.Id != songId)
            {
                return BadRequest(new ApiBadRequestResponse("Id passed in url does not match id passed in body."));
            }

            var songi = await Context.Song.FindAsync(songId);
            if (songi == null)
                return NotFound(new ApiResponse(404));
            try
            {
                Context.Entry(await Context.Song.FirstOrDefaultAsync(x => x.Id == songId)).CurrentValues.SetValues(song);
                await Context.SaveChangesAsync();
                song = await Context.Song.Include(x => x.Style).Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == songId);
                return Ok(new ApiOKResponse(song));
            }
            catch (Exception ex)
            {
                return Conflict(new ApiResponse(409, "There is already a Song with that name."));
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
                    Context.Song.Add(song);
                    await Context.SaveChangesAsync();
                    song = await Context.Song.Include(x => x.Style).Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == song.Id);
                }
                catch (DbUpdateException ex)
                {
                    return Conflict(new ApiResponse(409, "There is already a Song with that name."));
                }
                return Ok(new ApiOKResponse(song));
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
            var songItem = await Context.Song.Include(x => x.Style).Include(x => x.Band).FirstOrDefaultAsync(x => x.Id == songId);
            if (songItem == null)
                return NotFound(new ApiResponse(404));

            Context.Song.Remove(songItem);
            await Context.SaveChangesAsync();
            return Ok(new ApiOKResponse(songItem));
        }
    }
}