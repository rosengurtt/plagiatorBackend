using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SQLDBAccess.Models
{
    public class Style
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Band> Bands { get; set; }
        [JsonIgnore]
        public ICollection<Song> Songs { get; set; }
    }
}
