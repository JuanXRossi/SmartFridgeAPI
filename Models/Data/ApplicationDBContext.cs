using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SmartFridgeAPI.Models.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {}

        public DbSet<Urgency> Urgencies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Member",
                    NormalizedName = "MEMBER",
                    ConcurrencyStamp = "2"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}