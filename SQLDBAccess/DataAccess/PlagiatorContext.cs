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

        public DbSet<Style> Style { get; set; }
        public DbSet<Band> Band { get; set; }
        public DbSet<Song> Song { get; set; }
        public DbSet<TimeSignature> TimeSignature { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Style>().ToTable("Style");
            modelBuilder.Entity<Band>().ToTable("Band");
            modelBuilder.Entity<Song>().ToTable("Song");
            modelBuilder.Entity<TimeSignature>().ToTable("TimeSignature");
        }

    }


}
