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

        public DbSet<PitchPattern> PitchPatterns { get; set; }
        public DbSet<RythmPattern> RythmPatterns { get; set; }
        public DbSet<MelodyPattern> MelodyPatterns     { get; set; }
        public DbSet<MelodyPatternOccurrence> MelodyPatternOccurrences { get; set; }

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
            modelBuilder.Entity<PitchPattern>().ToTable("PitchPattern");
            modelBuilder.Entity<RythmPattern>().ToTable("RythmPattern");
            modelBuilder.Entity<MelodyPattern>().ToTable("MelodyPattern");
            modelBuilder.Entity<MelodyPatternOccurrence>().ToTable("MelodyPatternOccurrence");

            modelBuilder.Entity<Style>()
                .HasAlternateKey(c => c.Name).HasName("IX_StyleName");
            modelBuilder.Entity<Band>()
                .HasAlternateKey(c => c.Name).HasName("IX_BandName");

        }

    }


}
