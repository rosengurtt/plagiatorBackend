using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public async Task<List<Band>> GetBandsAsync(
                  int pageNo = 1,
                  int pageSize = 10000,
                  string startWith = null,
                  long? styleId = null)
        {
            if (styleId != null)
            {
                return await Context.Bands
                    .Where(x => x.Style.Id == styleId).OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else if (string.IsNullOrEmpty(startWith))
                return await Context.Bands.OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            else
                return await Context.Bands.OrderBy(x => x.Name)
                    .Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public async Task<int> GetNumberOfBandsAsync(
            string startWith = null,
            long? styleId = null)
        {
            if (styleId != null)
            {
                return await Context.Bands
                    .Where(x => x.Style.Id == styleId).OrderBy(x => x.Name).CountAsync();
            }
            else if (string.IsNullOrEmpty(startWith))
                return await Context.Bands.OrderBy(x => x.Name).CountAsync();
            else
                return await Context.Bands.OrderBy(x => x.Name)
                    .Where(x => x.Name.StartsWith(startWith)).CountAsync();
        }

        public async Task<Band> GetBandByIdAsync(long bandId)
        {
            return await Context.Bands.Include(x => x.Style)
                .FirstOrDefaultAsync(x => x.Id == bandId);
        }
        public async Task<Band> GetBandByNameAsync(string name)
        {
            return await Context.Bands.Where(b => b.Name == name).FirstOrDefaultAsync();
        }
        public async Task<Band> AddBandAsync(Band band)
        {
            Context.Bands.Add(band);
            await Context.SaveChangesAsync();
            return band;
        }

        public async Task<Band> UpdateBandAsync(Band band)
        {
            var bands = await Context.Bands.FindAsync(band.Id);
            if (bands == null)
                throw new ApplicationException($"No band with id {band.Id}");

            Context.Entry(await Context.Bands
                .FirstOrDefaultAsync(x => x.Id == band.Id))
                .CurrentValues.SetValues(band);
            await Context.SaveChangesAsync();
            return band;
        }

        public async Task DeleteBandAsync(long bandId)
        {
            var bandItem = await Context.Bands.FirstOrDefaultAsync(x => x.Id == bandId);
            if (bandItem == null)
                throw new ApplicationException($"No band with id {bandId}");

            Context.Bands.Remove(bandItem);
            await Context.SaveChangesAsync();
        }
    }
}
