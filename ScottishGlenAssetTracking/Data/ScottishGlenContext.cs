using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Models;

namespace ScottishGlenAssetTracking.Data
{
    /// <summary>
    /// ScottishGlenContext class used to interact with the database.
    /// </summary>
    public class ScottishGlenContext : DbContext
    {
        // DbSet properties for each entity.
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HardwareAsset> HardwareAssets { get; set; }

        /// <summary>
        /// Constructor for the ScottishGlenContext class.
        /// </summary>
        /// <param name="options">DBContext options used with dependency injection.</param>
        public ScottishGlenContext(DbContextOptions<ScottishGlenContext> options) : base(options)
        {
        }

        /// <summary>
        /// Override of the OnModelCreating method to configure the database schema.
        /// </summary>
        /// <param name="modelBuilder">Model builder to be used within the method.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Department entity.
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("SG.Departments");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).HasColumnName("name").HasMaxLength(64);
            });

            // Configure the Employee entity.
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("SG.Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasColumnName("firstName").HasMaxLength(64);
                entity.Property(e => e.LastName).HasColumnName("lastName").HasMaxLength(64);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(64);

                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees);
            });

            // Configure the HardwareAsset entity.
            modelBuilder.Entity<HardwareAsset>(entity =>
            {
                entity.ToTable("SG.HardwareAssets");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).HasColumnName("name").HasMaxLength(64);
                entity.Property(a => a.Model).HasColumnName("model").HasMaxLength(64);
                entity.Property(a => a.Manufacturer).HasColumnName("manufacturer").HasMaxLength(64);
                entity.Property(a => a.Type).HasColumnName("type").HasMaxLength(64);
                entity.Property(a => a.IpAddress).HasColumnName("ipAddress").HasMaxLength(64);
                entity.Property(a => a.PurchaseDate).HasColumnName("purchaseDate").IsRequired(false);
                entity.Property(a => a.Notes).HasColumnName("notes").HasMaxLength(64).IsRequired(false);

                entity.HasOne(a => a.Employee)
                      .WithMany(e => e.HardwareAssets);
            });
        }
    }
}
