using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly string ConnectionString;
        public Repository(PlagiatorContext context, IConfiguration configuration)
        {
            Context = context;
            ConnectionString = configuration.GetSection("ConnectionStrings:PlagiatorSql").Value;
        }

        public async Task<TimeSignature> GetTimeSignature(TimeSignature ts)
        {
            return await Context.TimeSignatures.Where(x => x.Numerator == ts.Numerator &
            x.Denominator == ts.Denominator).FirstOrDefaultAsync();
        }
    }
}
