using Microsoft.EntityFrameworkCore;
using Plagiator.Models.Entities;

namespace Plagiator.Persistence
{
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

        public DbSet<SongSimplification> SongSimplifications { get; set; }

        public DbSet<Pattern> Patterns { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }

        public DbSet<Chord> Chords { get; set; }
        public DbSet<ChordOccurrence> ChordOccurrences { get; set; }

        public DbSet<Melody> Melodies { get; set; }
        public DbSet<MelodyNote> MelodyNotes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Style>().ToTable("Styles");
            modelBuilder.Entity<Band>().ToTable("Bands");
            modelBuilder.Entity<Song>().ToTable("Songs");
            modelBuilder.Entity<TimeSignature>().ToTable("TimeSignatures");
            modelBuilder.Entity<Note>().ToTable("Notes");
            modelBuilder.Entity<Bar>().ToTable("Bars");
            modelBuilder.Entity<PitchBendItem>().ToTable("PitchBendItems");
            modelBuilder.Entity<TempoChange>().ToTable("TempoChanges");
            modelBuilder.Entity<SongSimplification>().ToTable("SongSimplifications");
            modelBuilder.Entity<Pattern>().ToTable("Patterns");
            modelBuilder.Entity<Occurrence>().ToTable("Occurrences");
            modelBuilder.Entity<Chord>().ToTable("Chords");
            modelBuilder.Entity<ChordOccurrence>().ToTable("ChordOccurrences");
            modelBuilder.Entity<Melody>().ToTable("Melodies");
            modelBuilder.Entity<MelodyNote>().ToTable("MelodyNotes");

            modelBuilder.Entity<Style>()
                .HasAlternateKey(c => c.Name).HasName("IX_StyleName");
            modelBuilder.Entity<Band>()
                .HasAlternateKey(c => c.Name).HasName("IX_BandName");


        }

    }


}
