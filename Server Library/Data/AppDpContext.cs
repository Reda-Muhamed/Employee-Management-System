using Base_Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Server_Library.Data
{
    public class AppDpContext:DbContext
    {
        // Constructor to pass DbContextOptions to the base class
        public AppDpContext(DbContextOptions<AppDpContext> options) : base(options)
        {
        }

        // Define DbSet properties for your entities here, e.g.:
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<GeneralDepartment> GeneralDepartments { get; set; }
        public DbSet<SystemRoles> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and properties here, e.g.:
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Town)
                .WithMany(t => t.Employees)
                .HasForeignKey(e => e.TownId);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.GeneralDepartment)
                .WithMany(gd => gd.Employees)
                .HasForeignKey(e => e.GeneralDepartmentId);
        }


    }
}
