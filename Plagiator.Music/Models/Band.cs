using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plagiator.Music.Models
{
    public class Band
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long StyleId { get; set; }
        public Style Style { get; set; }
    }
}
