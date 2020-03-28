using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    public partial class Repository : IRepository
    {
        private readonly PlagiatorContext Context;
        public Repository(PlagiatorContext context)
        {
            Context = context;
        }

        public async Task<TimeSignature> GetTimeSignature(TimeSignature ts)
        {
            return await Context.TimeSignatures.Where(x => x.Numerator == ts.Numerator &
            x.Denominator == ts.Denominator).FirstOrDefaultAsync();
        }
    }
}
