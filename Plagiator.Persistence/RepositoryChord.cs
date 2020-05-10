using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;
using Plagiator.Models.enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plagiator.Persistence
{
    partial class Repository
    {
        public Chord AddChord(Chord chord)
        {
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {
                sqlCnn.Open();
                using (var sqlCmd = new SqlCommand(@$"insert into 
                        Chords(PitchesAsString, PitchLettersAsString, IntervalsAsString) 
                        values (@pitchesAsString, @pitchLettersAsString, @intervalsAsString) 
                        SELECT SCOPE_IDENTITY();", sqlCnn))
                {
                    var pitchesAsString = new SqlParameter()
                    {
                        ParameterName = "@pitchesAsString",
                        DbType = DbType.String,
                        Value = chord.PitchesAsString
                    };
                    sqlCmd.Parameters.Add(pitchesAsString);
                    var pitchLettersAsString = new SqlParameter()
                    {
                        ParameterName = "@pitchLettersAsString",
                        DbType = DbType.String,
                        Value = chord.PitchLettersAsString
                    };
                    sqlCmd.Parameters.Add(pitchLettersAsString);
                    var intervalsAsString = new SqlParameter()
                    {
                        ParameterName = "@intervalsAsString",
                        DbType = DbType.String,
                        Value = chord.IntervalsAsString
                    };
                    sqlCmd.Parameters.Add(intervalsAsString);
                    chord.Id = Convert.ToInt64(sqlCmd.ExecuteScalar());
                }
            }
            return chord;
        }


        public ChordOccurrence AddChordOccurence(ChordOccurrence chordoc)
        {
            using (var sqlCnn = new SqlConnection(ConnectionString))
            {
                sqlCnn.Open();
                using (var sqlCmd = new SqlCommand(@$"insert into 
                        ChordOccurrences(ChordId, SongSimplificationId, StartTick, EndTick) 
                        values (@ChordId, @SongSimplificationId, @StartTick, @EndTick) 
                        SELECT SCOPE_IDENTITY();", sqlCnn))
                {
                    var chordId = new SqlParameter()
                    {
                        ParameterName = "@ChordId",
                        DbType = DbType.Int64,
                        Value = chordoc.ChordId
                    };
                    sqlCmd.Parameters.Add(chordId);
                    var songSimplificationId = new SqlParameter()
                    {
                        ParameterName = "@SongSimplificationId",
                        DbType = DbType.Int64,
                        Value = chordoc.SongSimplificationId
                    };
                    sqlCmd.Parameters.Add(songSimplificationId);
                    var startTick = new SqlParameter()
                    {
                        ParameterName = "@StartTick",
                        DbType = DbType.Int64,
                        Value = chordoc.StartTick
                    };
                    sqlCmd.Parameters.Add(startTick);
                    var endTick = new SqlParameter()
                    {
                        ParameterName = "@EndTick",
                        DbType = DbType.Int64,
                        Value = chordoc.EndTick
                    };
                    sqlCmd.Parameters.Add(endTick);
                    chordoc.Id = Convert.ToInt64(sqlCmd.ExecuteScalar());
                }
            }
            return chordoc;
        }


        public async Task<Chord> GetChordByIdAsync(int chordId)
        {
            return await Context.Chords.FindAsync(chordId);
        }


        public async Task<Chord> GetChordByStringAsync(string pitchesAsString)
        {
            return await Context.Chords
                .Where(a => a.PitchesAsString == pitchesAsString).FirstOrDefaultAsync();
        }
    }
}
