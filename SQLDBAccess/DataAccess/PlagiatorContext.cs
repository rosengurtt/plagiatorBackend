using Microsoft.EntityFrameworkCore;
using Plagiator.Music.Models;

namespace SQLDBAccess.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class PlagiatorContext : DbContext
    {
        public PlagiatorContext(DbContextOptions<PlagiatorContext> options) : base(options)
        {

        }

        public DbSet<Style> Styles { get; set; }
        public DbSet<Band> Bands { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<TimeSignature> TimeSignatures { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Bar> Bars { get; set; }
        public DbSet<PitchBendItem> PitchBendItems { get; set; }

        public DbSet<TempoChange> TempoChanges { get; set; }

        public DbSet<SongVersion> SongVersions { get; set; }

        public DbSet<Arpeggio> Arpeggios { get; set; }
        public DbSet<ArpeggioOccurrence> ArpeggioOccurrences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Style>().ToTable("Style");
            modelBuilder.Entity<Band>().ToTable("Band");
            modelBuilder.Entity<Song>().ToTable("Song");
            modelBuilder.Entity<TimeSignature>().ToTable("TimeSignature");
            modelBuilder.Entity<Note>().ToTable("Note");
            modelBuilder.Entity<Bar>().ToTable("Bar");
            modelBuilder.Entity<PitchBendItem>().ToTable("PitchBendItem");
            modelBuilder.Entity<TempoChange>().ToTable("TempoChange");
            modelBuilder.Entity<SongVersion>().ToTable("SongVersion");
            modelBuilder.Entity<Arpeggio>().ToTable("Arpeggio");
            modelBuilder.Entity<ArpeggioOccurrence>().ToTable("ArpeggioOccurrence");

            modelBuilder.Entity<Style>()
                .HasAlternateKey(c => c.Name).HasName("IX_StyleName");
            modelBuilder.Entity<Band>()
                .HasAlternateKey(c => c.Name).HasName("IX_BandName");

            //modelBuilder.Entity<TimeSignature>()
            //    .HasAlternateKey(c => new { c.Numerator, c.Denominator })
            //    .HasName("IX_TimeSignature");
            //modelBuilder.Entity<Arpeggio>()
            //    .HasAlternateKey(c => new { c.PitchPatternString, c.RythmPatternString })
            //    .HasName("IX_Arpeggio_UniquePatterns");
            //modelBuilder.Entity<ArpeggioOccurrence>()
            //    .HasAlternateKey(c => new { c.ArpeggioId, c.SongVersionId })
            //    .HasName("IX_ArpeggioOccurrence_UniqueArpSong");
        }

    }


}
