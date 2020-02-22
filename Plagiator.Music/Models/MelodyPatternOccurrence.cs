using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Plagiator.Music.Models
{
    //public class MelodyPatternOccurrence
    //{
    //    public long Id { get; set; }

    //    public int SongVersionId { get; set; }

    //    public SongVersion SongVersion { get; set; }

    //    public int MelodyPatternId { get; set; }

    //    public MelodyPattern MelodyPattern { get; set; }

    //    /// <summary>
    //    /// The places in the song where they are used
    //    /// </summary>
    //    [NotMapped]
    //    public List<SongInterval> Occurrences { get; set; }

    //    public string OccurrencesString
    //    {
    //        get
    //        {
    //            return Occurrences.Aggregate("", (ac, next) => ac + "," + next.AsString());
    //        }
    //        set
    //        {
    //            Occurrences = Array.ConvertAll(value.Split(","), s => SongInterval.FromString(s)).ToList();
    //        }
    //    }
    //}
}
