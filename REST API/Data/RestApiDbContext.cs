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

            modelBuilder.Entity<User>(entity => entity.HasIndex(u => u.Name).IsUnique());
            modelBuilder.Entity<User>(entity => entity.HasIndex(u => u.Email).IsUnique());

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "m",
                Email = "m@m.com",
                Password = "m",
                Role = "Admin"
            });
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                Name = "i",
                Email = "i@i.com",
                Password = "i",
                Role = "User"
            });
        }

        public DbSet<User> Users { get; set; }
    }
}
