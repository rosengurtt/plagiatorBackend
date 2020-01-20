using Microsoft.AspNetCore.Cors;
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
    public class StyleController : ControllerBase
    {
        private readonly PlagiatorContext Context;

        public StyleController(PlagiatorContext context)
        {
            Context = context;
        }
        [EnableCors]
        [HttpGet]
        public async Task<ActionResult<IEnumerable>> GetStyles(int pageNo = 1, int pageSize = 10, string startWith = null)
        {
            if (string.IsNullOrEmpty(startWith))
                return await Context.Style.OrderBy(x => x.Name).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Style.OrderBy(x => x.Name).Where(x => x.Name.StartsWith(startWith)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: api/Style/5
        [HttpGet("{styleId}")]
        public async Task<IActionResult> GetStyle(int styleId)
        {
            var styles = await Context.Style.FindAsync(styleId);

            if (styles == null)
            {
                return NotFound(new ApiResponse(404));
            }

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

            var styles = await Context.Style.FindAsync(styleId);
            if (styles == null)
                return NotFound(new ApiResponse(404));
            try
            {
                Context.Entry(await Context.Style.FirstOrDefaultAsync(x => x.Id == styleId)).CurrentValues.SetValues(style);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict(new ApiResponse(409, "There is already a style with that name."));
            }
            return Ok(new ApiOKResponse(style));
        }

        // POST: api/Style
        [HttpPost]
        public async Task<ActionResult> PostStyle(Style style)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Style.Add(style);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return Conflict(new ApiResponse(409, "There is already a style with that name."));
                }
                return Ok(new ApiOKResponse(style));
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse(ModelState));
            }
        }

        // DELETE: api/Style/5
        [HttpDelete("{styleId}")]
        public async Task<ActionResult> DeleteStyle(int styleId)
        {
            var styleItem = await Context.Style.FindAsync(styleId);
            if (styleItem == null)
                return NotFound(new ApiResponse(404));

            Context.Style.Remove(styleItem);
            await Context.SaveChangesAsync();
            return Ok(new ApiOKResponse(styleItem));
        }
    }
}
