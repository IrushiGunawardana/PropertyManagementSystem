using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Models;
using PropertyManagementSystem.Models.Schema;

namespace PropertyManagementSystem.Configurations
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyManager> PropertyManagers { get; set; }
        public DbSet<PropertyOwner> PropertyOwners { get; set; }
        public DbSet<PropertyTenant> PropertyTenants { get; set; }
        public DbSet<Models.Schema.ServiceProvider> ServiceProviders { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<ServiceProviderJobType> ServiceProviderJobTypes { get; set; }
        public DbSet<Job> Jobs { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User to PropertyManager (One-to-Many)
            modelBuilder.Entity<PropertyManager>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.PropertyManagers)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PropertyManager to Property (One-to-Many)
            modelBuilder.Entity<PropertyManager>()
                .HasOne(pm => pm.Property)
                .WithMany(p => p.PropertyManagers)
                .HasForeignKey(pm => pm.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to PropertyOwner (One-to-Many)
            modelBuilder.Entity<PropertyOwner>()
                .HasOne(po => po.User)
                .WithMany(u => u.PropertyOwners)
                .HasForeignKey(po => po.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PropertyOwner to Property (One-to-Many)
            modelBuilder.Entity<PropertyOwner>()
                .HasOne(po => po.Property)
                .WithMany(p => p.PropertyOwners)
                .HasForeignKey(po => po.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to PropertyTenant (One-to-Many)
            modelBuilder.Entity<PropertyTenant>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PropertyTenants)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PropertyTenant to Property (One-to-Many)
            modelBuilder.Entity<PropertyTenant>()
                .HasOne(pt => pt.Property)
                .WithMany(p => p.PropertyTenants)
                .HasForeignKey(pt => pt.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // User to ServiceProvider (One-to-Many)
            modelBuilder.Entity<Models.Schema.ServiceProvider>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.ServiceProviders)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceProviderJobType to ServiceProvider (Many-to-One)
            modelBuilder.Entity<ServiceProviderJobType>()
                .HasOne(spjt => spjt.ServiceProvider)
                .WithMany(sp => sp.ServiceProviderJobTypes)
                .HasForeignKey(spjt => spjt.ServiceProviderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceProviderJobType to JobType (Many-to-One)
            modelBuilder.Entity<ServiceProviderJobType>()
                .HasOne(spjt => spjt.JobType)
                .WithMany(jt => jt.ServiceProviderJobTypes)
                .HasForeignKey(spjt => spjt.JobTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Job to Property (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Property)
                .WithMany(p => p.Jobs)
                .HasForeignKey(j => j.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Job to User (PostedBy) (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.PostedByUser)
                .WithMany(u => u.PostedJobs)
                .HasForeignKey(j => j.PostedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Job to JobType (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.JobType)
                .WithMany(jt => jt.Jobs)
                .HasForeignKey(j => j.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Job to ServiceProvider (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.ServiceProvider)
                .WithMany(sp => sp.Jobs)
                .HasForeignKey(j => j.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

   
}
