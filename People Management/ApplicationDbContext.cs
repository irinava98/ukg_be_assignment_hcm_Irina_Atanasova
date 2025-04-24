using Microsoft.EntityFrameworkCore;

namespace People_Management
{
   public class ApplicationDbContext: DbContext
   {
      public DbSet<Person> People { get; set; }

      public DbSet<AppUser> Users { get; set; } = null!;


      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
      {
      }

      protected override void OnConfiguring(
      DbContextOptionsBuilder optionsBuilder)
      {
         optionsBuilder.UseInMemoryDatabase("PeopleDb");
      }
   }
}
