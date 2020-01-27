using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLDBAccess.DataAccess;
using SQLDBAccess.ErrorHandling;
using Plagiator.Music.Models;
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
        private ISongRepository SongRepository;

        public BandController( ISongRepository SongRepository)
        {
            this.SongRepository = SongRepository;
        }
        // GET: api/Band?pageSize=10&pageNo=2
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetBands(
            int pageNo = 1,
            int pageSize = 10000,
            string startWith = null,
            int? styleId = null)
        {
            var totalBands = await SongRepository.GetNumberOfBands(pageNo, pageSize, startWith, styleId);

            var bands = await SongRepository.GetBands(pageNo, pageSize, startWith, styleId);
            var retObj = new
            {
                page = pageNo,
                totalPages = (int)Math.Ceiling((double)totalBands / pageSize),
                bands
            };
            return Ok(new ApiOKResponse(retObj));
        }

        // GET: api/Band/5
        [HttpGet("{bandId}")]
        public async Task<IActionResult> GetBand(int bandId)
        {
            var band = await SongRepository.GetBandById(bandId);

            if (band == null)
                return NotFound(new ApiResponse(404, $"No band with id {bandId}"));

            return Ok(new ApiOKResponse(band));
        }

        // PUT: api/Band/5
        [HttpPut("{bandId}")]
        public async Task<ActionResult> PutBand(int bandId, Band band)
        {
            if (band.Id != bandId)
                return BadRequest(new ApiBadRequestResponse("Id passed in url does not match id passed in body."));
            
            try
            {
                var bandita = await SongRepository.UpdateBand(band);
                return Ok(new ApiOKResponse(bandita));
            }
            catch (ApplicationException)
            {
                return NotFound(new ApiResponse(404, $"No band with id {bandId}"));
            }
        }

        // POST: api/Band
        [HttpPost]
        public async Task<ActionResult> PostBand(Band band)
        {
            if (ModelState.IsValid)
            {
                var bandita = await SongRepository.AddBand(band);
                return Ok(new ApiOKResponse(bandita));
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
            try
            {
                await SongRepository.DeleteBand(bandId);
                return Ok(new ApiOKResponse("Record deleted"));
            }
            catch (ApplicationException) {
                return NotFound(new ApiResponse(404, $"No band with id {bandId}"));
            }
        }
    }
}
