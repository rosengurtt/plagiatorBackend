using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    public partial class Repository : IRepository
    {
        public async Task<Pattern> GetPatternByIdAsync(long patternId)
        {
            return await Context.Patterns.FindAsync(patternId);
        }


        /// <summary>
        /// This one uses ado.net because EF is a peace of sheet. Thank you Machosoft for your
        /// crapp software
        /// </summary>
        /// <param name="patternString"></param>
        /// <param name="patternType"></param>
        /// <returns></returns>
        public Pattern GetPatternByStringAndType(string patternString, PatternType patternType)
        {
            DataSet ds = new DataSet();
            var sqlCnn = new SqlConnection(ConnectionString);
            try
            {
                sqlCnn.Open();
                var sqlCmd = new SqlCommand("Select * from Patterns where AsString=@AsString and PatternTypeId=@TypeId", sqlCnn);
                var adapter = new SqlDataAdapter() { SelectCommand = sqlCmd };
                var paramPattern = new SqlParameter()
                {
                    ParameterName = "@AsString",
                    DbType = DbType.String,
                    Value = patternString
                };
                var paramType = new SqlParameter()
                {
                    ParameterName = "@TypeId",
                    DbType = DbType.Int32,
                    Value = (int)patternType
                };
                adapter.Fill(ds);
                if (ds!=null && ds.Tables!=null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count == 0) return null;
                    else
                    {
                        var pat = new Pattern()
                        {
                            AsString = patternString,
                            PatternTypeId = patternType,
                            Id = (int)ds.Tables[0].Rows[0].ItemArray[0]
                        };
                    }
                }
                adapter.Dispose();
                sqlCmd.Dispose();
                sqlCnn.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception was raised when trying to get a pattern using ado.net");
            }
            return null;
        }
        public Pattern AddPattern(Pattern pattern)
        {
            return Context.Patterns
                  .FromSqlRaw($"insert into Patterns(AsString, PatternTypeId) values ('{pattern.AsString}', {(int)pattern.PatternTypeId}) SELECT * FROM Patterns WHERE id = SCOPE_IDENTITY();")
                  .ToList().FirstOrDefault();
        }

        
        public async Task<Occurrence> GetOccurrenceByIdAsync(long ocId)
        {
            return await Context.Occurrences.FindAsync(ocId);
        }

        public async Task<List<Occurrence>> GetPatternOccurrencesOfSongSimplification(long songSimplificationId)
        {
            return await Context.Occurrences
                .Where(x => x.SongSimplificationId == songSimplificationId)
                .Include(y=>y.Pattern).ToListAsync();
        }

        public async Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternId(
            long SongSimplificationId, long patternId)
        {

            var occurs =  await Context.Occurrences.Join(
                Context.SongSimplifications,
                occurrence => occurrence.SongSimplificationId,
                songVersion => songVersion.Id,
                (occurrence, songSimplification) => new Occurrence
                {
                    Id = occurrence.Id,
                    SongSimplificationId = songSimplification.Id,
                    PatternId = occurrence.PatternId,
                    Pattern = occurrence.Pattern
                }
                )
                .Where(a => a.SongSimplificationId == SongSimplificationId & a.PatternId == patternId).ToListAsync();
            foreach(var oc in occurs)
            {
                oc.Notes = await Context.Notes.Join(
                    Context.OccurrenceNotes.Where(occur=>occur.OccurrenceId==oc.Id),
                    note => note.Id,
                    occurrenceNote => occurrenceNote.NoteId,
                    (note, occurrenceNote) => note).ToListAsync();
            }
            return occurs;
        }

        public async Task<Occurrence> AddOccurrence(Occurrence oc)
        {
            Context.Occurrences.Add(oc);
            foreach (var note in oc.Notes)
            {
                Context.OccurrenceNotes.Add(new OccurrenceNote()
                {
                    OccurrenceId = oc.Id,
                    NoteId = note.Id
                });
            }
            await Context.SaveChangesAsync();
            return oc;

        }
    }
}
