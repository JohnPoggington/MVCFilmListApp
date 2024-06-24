using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCFilmLists.Models;
using System.Reflection.Emit;

namespace MVCFilmLists.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MVCFilmLists.Models.Director> Director { get; set; } = default!;
        public DbSet<MVCFilmLists.Models.Genre> Genre { get; set; } = default!;
        public DbSet<MVCFilmLists.Models.Movie> Movie { get; set; } = default!;

        public DbSet<ApplicationUser> User { get; set; } = default!;

        public DbSet<MovieList> MovieLists { get; set; } = default!;

        //public DbSet<ListEntry> ListEntries { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<MovieList>()
            //    .HasMany(m => m.Movies)
            //    .WithMany(l => l.movieLists)
            //    .UsingEntity<ListEntry>(
            //    j => j.Property(e => e.AddedOn).HasDefaultValueSql("CURRENT_TIMESTAMP")
            //    );
            ////builder.Entity<ListEntry>(e => e.Property(p => p.AddedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
            builder.Entity<MovieList>(l => l.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP"));

            //builder.Entity<ListEntry>()
            //    .HasKey(c => new { c.MovieListId, c.MovieId });

        }
        public DbSet<MVCFilmLists.Models.Review> Review { get; set; } = default!;
    }
}
