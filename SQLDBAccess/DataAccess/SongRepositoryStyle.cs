using Microsoft.EntityFrameworkCore;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.DataAccess
{
    public partial class SongRepository : ISongRepository
    {

        public async Task<Style> GetStyleById(int styleId)
        {
            return await Context.Style.FindAsync(styleId);
        }
        public async Task<Style> GetStyleByName(string name)
        {
            return await Context.Style.Where(s => s.Name == name).FirstOrDefaultAsync();
        }
        public async Task<Style> AddStyle(Style style)
        {
            Context.Style.Add(style);
            await Context.SaveChangesAsync();
            return style;
        }

        public async Task<Style> UpdateStyle(Style style)
        {
            var styles = await Context.Style.FindAsync(style.Id);
            if (styles == null)
                throw new ApplicationException($"No style with id {style.Id}");

            Context.Entry(await Context.Style.FirstOrDefaultAsync(x => x.Id == style.Id))
                    .CurrentValues.SetValues(style);
            await Context.SaveChangesAsync();
            return style;
        }

        public async Task DeleteStyle(int styleId)
        {
            var style = await Context.Style.FindAsync(styleId);
            if (style == null)
                throw new ApplicationException("There is no style with that id");

            Context.Style.Remove(style);
            await Context.SaveChangesAsync();
        }

    }
}
