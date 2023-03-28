using Microsoft.EntityFrameworkCore;
using REST_API.Entities;

namespace REST_API.Data
{
    public class RestApiDbContext : DbContext
    {
        public RestApiDbContext(DbContextOptions<RestApiDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Mihai",
                Password = "Parola",
                Role = "Owner"
            });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                Username = "Ioana",
                Password = "Parola",
                Role = "Guest"
            });
        }

        public DbSet<User> Users { get; set; }
    }
}
