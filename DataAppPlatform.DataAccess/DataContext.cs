using System;
using DataAppPlatform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}