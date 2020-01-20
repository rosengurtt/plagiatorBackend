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
    public class BandController : ControllerBase
    {
        private readonly PlagiatorContext Context;

        public BandController(PlagiatorContext context)
        {
            Context = context;
        }
        // GET: api/Band?pageSize=10&pageNo=2
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetBands(
            int pageNo = 1,
            int pageSize = 10000,
            string startWith = null,
            int? styleId = null)
        {
            if (styleId != null)
            {
                return await Context.Band.Where(x => x.Style.Id == styleId).OrderBy(x => x.Name).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else if (string.IsNullOrEmpty(startWith))
                return await Context.Band.OrderBy(x => x.Name).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Band.OrderBy(x => x.Name).Where(x => x.Name.StartsWith(startWith)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/Band/5
        [HttpGet("{bandId}")]
        public async Task<IActionResult> GetBand(int bandId)
        {
            var band = await Context.Band.Include(x => x.Style).FirstOrDefaultAsync(x => x.Id == bandId);

            if (band == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(new ApiOKResponse(band));
        }

        // PUT: api/Band/5
        [HttpPut("{bandId}")]
        public async Task<ActionResult> PutBand(int bandId, Band band)
        {
            if (band.Id != bandId)
            {
                return BadRequest(new ApiBadRequestResponse("Id passed in url does not match id passed in body."));
            }

            var bands = await Context.Band.FindAsync(bandId);
            if (bands == null)
                return NotFound(new ApiResponse(404));
            try
            {
                Context.Entry(await Context.Band.FirstOrDefaultAsync(x => x.Id == bandId)).CurrentValues.SetValues(band);
                await Context.SaveChangesAsync();
                band = await Context.Band.Include(x => x.Style).FirstOrDefaultAsync(x=>x.Id== bandId);
                return Ok(new ApiOKResponse(band));
            }
            catch (Exception ex)
            {
                return Conflict(new ApiResponse(409, "There is already a band with that name."));
            }
        }

        // POST: api/Band
        [HttpPost]
        public async Task<ActionResult> PostBand(Band band)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Band.Add(band);
                    await Context.SaveChangesAsync();
                    band = await Context.Band.Include(x => x.Style).FirstOrDefaultAsync(x => x.Id == band.Id);
                }
                catch (DbUpdateException ex)
                {
                    return Conflict(new ApiResponse(409, "There is already a band with that name."));
                }
                return Ok(new ApiOKResponse(band));
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse(ModelState));
            }
        }

        // DELETE: api/Band/5
        [HttpDelete("{bandId}")]
        public async Task<ActionResult> DeleteBand(int bandId)
        {
            var bandItem = await Context.Band.Include(x=>x.Style).FirstOrDefaultAsync(x => x.Id == bandId);
            if (bandItem == null)
                return NotFound(new ApiResponse(404));

            Context.Band.Remove(bandItem);
            await Context.SaveChangesAsync();
            return Ok(new ApiOKResponse(bandItem));
        }
    }
}
