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
            if (Database.EnsureCreated())
            {
                AddSampleData();
            }
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                AddSampleData();
            }
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

        private void AddSampleData()
        {
            var contact1 = new Contact()
            {
                FirstName = "Mark",
                LastName = "Ivanov",
                BirthDate = new DateTime(1970, 3, 15),
                Email = "ivanov@gmail.com",
                Phone = "79171573840"
            };

            Contacts.Add(new Contact()
            {
                FirstName = "Eugene",
                LastName = "Yalandaev",
                BirthDate = new DateTime(1990, 9, 8),
                Email = "yalandaev@gmail.com",
                Phone = "79171571681",
                Manager = contact1
            });
            Contacts.Add(new Contact()
            {
                FirstName = "Angela",
                LastName = "Yalandaev",
                BirthDate = new DateTime(1989, 12, 6),
                Email = "yalandaeva@gmail.com",
                Phone = "79171571362",
                Manager = contact1
            });
            Contacts.Add(new Contact()
            {
                FirstName = "Frank",
                LastName = "Maslov",
                BirthDate = new DateTime(1985, 3, 1),
                Email = "maslov@gmail.com",
                Phone = "79171574421",
                Manager = contact1
            });
            SaveChanges();
        }
    }
}