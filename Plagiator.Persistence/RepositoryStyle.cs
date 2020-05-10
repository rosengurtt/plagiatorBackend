using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class  Repository
    {

        public async Task<List<Style>> GetStylesAsync(
            int pageNo = 1,
            int pageSize = 10,
            string startWith = null)
        {
            if (string.IsNullOrEmpty(startWith))
                return await Context.Styles.OrderBy(x => x.Name).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Styles.OrderBy(x => x.Name).Where(x => x.Name.StartsWith(startWith)).Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<int> GetNumberOfStylesAsync(
            int pageNo = 1,
            int pageSize = 10,
            string startWith = null)
        {
            if (string.IsNullOrEmpty(startWith))
                return await Context.Styles.OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
            else
                return await Context.Styles.OrderBy(x => x.Name)
                    .Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
        }
        public async Task<Style> GetStyleByIdAsync(long styleId)
        {
            return await Context.Styles.FindAsync(styleId);
        }
        public async Task<Style> GetStyleByNameAsync(string name)
        {
            return await Context.Styles.Where(s => s.Name == name).FirstOrDefaultAsync();
        }
        public async Task<Style> AddStyleAsync(Style style)
        {
            Context.Styles.Add(style);
            await Context.SaveChangesAsync();
            return style;
        }

        public async Task<Style> UpdateStyleAsync(Style style)
        {
            var styles = await Context.Styles.FindAsync(style.Id);
            if (styles == null)
                throw new ApplicationException($"No style with id {style.Id}");

            Context.Entry(await Context.Styles.FirstOrDefaultAsync(x => x.Id == style.Id))
                    .CurrentValues.SetValues(style);
            await Context.SaveChangesAsync();
            return style;
        }

        public async Task DeleteStyleAsync(long styleId)
        {
            var style = await Context.Styles.FindAsync(styleId);
            if (style == null)
                throw new ApplicationException("There is no style with that id");

            Context.Styles.Remove(style);
            await Context.SaveChangesAsync();
        }
    }
}
