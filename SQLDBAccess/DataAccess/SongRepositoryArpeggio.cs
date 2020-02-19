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
        public async Task<Arpeggio> GetArpeggioById(int arpeggioId)
        {
            return await Context.Arpeggios.FindAsync(arpeggioId);
        }
        public async Task<Arpeggio> GetArpeggioByPitchPatternIdAndRythmPatternId(
            int pitchPatternId,
            int rythmPatternId)
        {
            return await Context.Arpeggios
                .Where(a => a.PitchPatternId== pitchPatternId &
                a.RythmPatternId== rythmPatternId).FirstOrDefaultAsync();
        }
        public async Task<Arpeggio> AddArpeggio(Arpeggio arpeggio)
        {
            Context.Arpeggios.Add(arpeggio);
            await Context.SaveChangesAsync();
            return arpeggio;
        }

        public async Task<ArpeggioOccurrence> GetArpeggioOccurrenceById(int arpOcId)
        {
            return await Context.ArpeggioOccurrences.FindAsync(arpOcId);
        }
        public async Task<ArpeggioOccurrence> GetArpeggioOccurrencesForSongVersionIdAndArpeggioId(
            int songVersionId,
            int arpeggioId)
        {
            return await Context.ArpeggioOccurrences
                .Where(a => a.SongVersionId== songVersionId &
                a.ArpeggioId==arpeggioId).FirstOrDefaultAsync();
        }
        public async Task<ArpeggioOccurrence> AddArpeggioOccurrence(ArpeggioOccurrence arpOc)
        {
            Context.ArpeggioOccurrences.Add(arpOc);
            await Context.SaveChangesAsync();
            return arpOc;
        }
    }
}
