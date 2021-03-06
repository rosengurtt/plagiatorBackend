﻿using Microsoft.EntityFrameworkCore;
using Plagiator.Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.DataAccess
{
    public partial class SongRepository : ISongRepository
    {

        public async Task<List<Band>> GetBands(
            int pageNo = 1,
            int pageSize = 10000,
            string startWith = null,
            int? styleId = null)
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
        public async Task<int> GetNumberOfBands(
            int pageNo = 1,
            int pageSize = 10000,
            string startWith = null,
            int? styleId = null)
        {
            if (styleId != null)
            {
                return await Context.Bands
                    .Where(x => x.Style.Id == styleId).OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
            }
            else if (string.IsNullOrEmpty(startWith))
                return await Context.Bands.OrderBy(x => x.Name)
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
            else
                return await Context.Bands.OrderBy(x => x.Name)
                    .Where(x => x.Name.StartsWith(startWith))
                    .Skip((pageNo - 1) * pageSize).Take(pageSize).CountAsync();
        }

        public async Task<Band> GetBandById(int bandId)
        {
            return await Context.Bands.Include(x => x.Style)
                .FirstOrDefaultAsync(x => x.Id == bandId);
        }
        public async Task<Band> GetBandByName(string name)
        {
            return await Context.Bands.Where(b => b.Name == name).FirstOrDefaultAsync();
        }
        public async Task<Band> AddBand(Band band)
        {
            Context.Bands.Add(band);
            await Context.SaveChangesAsync();
            return band;
        }

        public async Task<Band> UpdateBand(Band band)
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

        public async Task DeleteBand(int bandId)
        {
            var bandItem = await Context.Bands.FirstOrDefaultAsync(x => x.Id == bandId);
            if (bandItem == null)
                throw new ApplicationException($"No band with id {bandId}");

            Context.Bands.Remove(bandItem);
            await Context.SaveChangesAsync();
        }

    }
}
