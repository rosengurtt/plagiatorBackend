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
        public async Task<TimeSignature> GetTimeSignature(TimeSignature ts)
        {
            return await Context.TimeSignatures.Where(x => x.Numerator == ts.Numerator &
            x.Denominator == ts.Denominator).FirstOrDefaultAsync();         
        }
    }
}
