using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.Models
{
    public class Band
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StyleId { get; set; }
        public Style Style { get; set; }
    }
}
