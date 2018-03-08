using DataAppPlatform.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAppPlatform.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.EnsureCreated();
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=DataAppPlatform;Persist Security Info=True;User ID=sa;Password=password");
        }
    }
}