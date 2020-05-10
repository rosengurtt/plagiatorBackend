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
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {
                sqlCnn.Open();
                using (var sqlCmd = new SqlCommand(@"Select Id
                                                    from Patterns 
                                                    where AsString = @AsString and 
                                                    PatternTypeId = @TypeId", sqlCnn))
                {
                    using (var adapter = new SqlDataAdapter() { SelectCommand = sqlCmd })
                    {
                        var paramPattern = new SqlParameter()
                        {
                            ParameterName = "@AsString",
                            DbType = DbType.String,
                            Value = patternString
                        };
                        sqlCmd.Parameters.Add(paramPattern);
                        var paramType = new SqlParameter()
                        {
                            ParameterName = "@TypeId",
                            DbType = DbType.Int32,
                            Value = (int)patternType
                        };
                        sqlCmd.Parameters.Add(paramType);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count == 0) return null;
                            else
                            {
                                var pat = new Pattern()
                                {
                                    AsString = patternString,
                                    PatternTypeId = patternType,
                                    Id = (long)ds.Tables[0].Rows[0].ItemArray[0]
                                };
                                return pat;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public Pattern AddPattern(Pattern pattern)
        {
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {
                sqlCnn.Open();
                using (var sqlCmd = new SqlCommand(@"insert into Patterns(AsString, PatternTypeId)
                                              values (@AsString, @PatternTypeId);
                                              SELECT SCOPE_IDENTITY();", sqlCnn))
                {
                    var AsString = new SqlParameter()
                    {
                        ParameterName = "@AsString",
                        DbType = DbType.String,
                        Value = pattern.AsString
                    };
                    sqlCmd.Parameters.Add(AsString);
                    var paramPatternTypeId = new SqlParameter()
                    {
                        ParameterName = "@PatternTypeId",
                        DbType = DbType.Int32,
                        Value = pattern.PatternTypeId
                    };
                    sqlCmd.Parameters.Add(paramPatternTypeId);
                    pattern.Id = Convert.ToInt64(sqlCmd.ExecuteScalar());
                }
            }
            return pattern;
        }


        public async Task<Occurrence> GetOccurrenceByIdAsync(long ocId)
        {
            return await Context.Occurrences.FindAsync(ocId);
        }

        public bool AreOccurrencesForSongSimplificationAlreadyProcessed(long songSimplificationId)
        {
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {
                sqlCnn.Open();
                using (var sqlCmd = new SqlCommand(@"Select count(*) from Occurrences
                                              where SongSimplificationId=@id", sqlCnn))
                {
                    var adapter = new SqlDataAdapter() { SelectCommand = sqlCmd };
                    var paramId = new SqlParameter()
                    {
                        ParameterName = "@id",
                        DbType = DbType.Int64,
                        Value = songSimplificationId
                    };
                    sqlCmd.Parameters.Add(paramId);
                    var count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        public async Task<List<Occurrence>> GetPatternOccurrencesOfSongSimplificationAsync(long songSimplificationId)
        {
            return await Context.Occurrences
                .Where(x => x.SongSimplificationId == songSimplificationId)
                .Include(y => y.Pattern).ToListAsync();
        }

        public async Task<List<Occurrence>> GetOccurrencesForSongVersionIdAndPatternIdAsync(
            long SongSimplificationId, long patternId)
        {

            var occurs = await Context.Occurrences.Join(
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
            foreach (var oc in occurs)
            {
                oc.Notes = await Context.Notes.Join(
                    Context.OccurrenceNotes.Where(occur => occur.OccurrenceId == oc.Id),
                    note => note.Id,
                    occurrenceNote => occurrenceNote.NoteId,
                    (note, occurrenceNote) => note).ToListAsync();
            }
            return occurs;
        }

        public Occurrence AddOccurrence(Occurrence oc)
        {
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {

                sqlCnn.Open();
                using (var sqlCmd1 = new SqlCommand(@"insert into Occurrences(SongSimplificationId,PatternId)
                                              values (@SongSimplificationId, @PatternId);
                                              SELECT SCOPE_IDENTITY();", sqlCnn))
                {
                    var paramSongSimplificationId = new SqlParameter()
                    {
                        ParameterName = "@SongSimplificationId",
                        DbType = DbType.Int64,
                        Value = oc.SongSimplificationId
                    };
                    sqlCmd1.Parameters.Add(paramSongSimplificationId);
                    var paramPatternId = new SqlParameter()
                    {
                        ParameterName = "@PatternId",
                        DbType = DbType.Int64,
                        Value = oc.Pattern.Id
                    };
                    sqlCmd1.Parameters.Add(paramPatternId);
                    oc.Id = Convert.ToInt64(sqlCmd1.ExecuteScalar());
                }

                foreach (var note in oc.Notes)
                {
                    using (var sqlCmd2 = new SqlCommand(@"insert into OccurrenceNotes(OccurrenceId,NoteId)
                                              values (@OccurrenceId, @NoteId)", sqlCnn))
                    {
                        var paramOccurrenceId = new SqlParameter()
                        {
                            ParameterName = "@OccurrenceId",
                            DbType = DbType.Int64,
                            Value = oc.Id
                        };
                        sqlCmd2.Parameters.Add(paramOccurrenceId);
                        var paramNoteId = new SqlParameter()
                        {
                            ParameterName = "@NoteId",
                            DbType = DbType.Int64,
                            Value = note.Id
                        };
                        sqlCmd2.Parameters.Add(paramNoteId);
                        sqlCmd2.ExecuteNonQuery();
                    }
                }
                return oc;
            }
        }
    }
}
