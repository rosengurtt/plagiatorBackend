using Microsoft.AspNetCore.Cors;
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
    public class StyleController : ControllerBase
    {
        private ISongRepository SongRepository;

        public StyleController(ISongRepository SongRepository)
        {
            this.SongRepository = SongRepository;
        }
        [EnableCors]
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetStyles(int pageNo = 1, int pageSize = 10, string startWith = null)
        {
            var totaStyles = await SongRepository.GetNumberOfStyles(pageNo, pageSize, startWith);
            var styles = await SongRepository.GetStyles(pageNo, pageSize, startWith);
            var retObj = new
            {
                page = pageNo,
                totalPages = (int)Math.Ceiling((double)totaStyles / pageSize),
                styles
            };
            return Ok(new ApiOKResponse(retObj));
        }

        // GET: api/Style/5
        [HttpGet("{styleId}")]
        public async Task<IActionResult> GetStyle(int styleId)
        {
            var styles = await SongRepository.GetStyleById(styleId);

            if (styles == null)
                return NotFound(new ApiResponse(404));

            return Ok(new ApiOKResponse(styles));
        }

        // PUT: api/Style/5
        [HttpPut("{styleId}")]
        public async Task<ActionResult> PutStyle(int styleId, Style style)
        {
            if (style.Id != styleId)
            {
                return BadRequest(new ApiBadRequestResponse("Id passed in url does not match id passed in body."));
            }

            try
            {
                var stylete = await SongRepository.UpdateStyle(style);
                return Ok(new ApiOKResponse(stylete));
            }
            catch (ApplicationException)
            {
                return NotFound(new ApiResponse(404, "No style with that id"));
            }
        }

        // POST: api/Style
        [HttpPost]
        public async Task<ActionResult> PostStyle(Style style)
        {
            if (ModelState.IsValid)
            {
                    var stylete = await SongRepository.AddStyle(style);
                    return Ok(new ApiOKResponse(stylete));               
            }
            else
                return BadRequest(new ApiBadRequestResponse(ModelState));
        }

        // DELETE: api/Style/5
        [HttpDelete("{styleId}")]
        public async Task<ActionResult> DeleteStyle(int styleId)
        {
            try
            {
                await SongRepository.DeleteStyle(styleId);
                return Ok(new ApiOKResponse("Record deleted"));
            }
            catch (ApplicationException)
            {
                return NotFound(new ApiResponse(404, "No style with that id"));
            }
        }
    }
}
