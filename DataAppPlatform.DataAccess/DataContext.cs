using DataAppPlatform.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAppPlatform.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Contact> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=dataappplatform;Trusted_Connection=True;");
        }
    }
}