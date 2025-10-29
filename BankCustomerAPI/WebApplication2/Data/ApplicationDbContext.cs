using Microsoft.EntityFrameworkCore;
using WebApplication2.Models.Entities;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<NormalUser> NormalUsers { get; set; }
        public DbSet<ViewOnlyUser> ViewOnlyUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<SYSAdmin> SYSAdmins { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<TermDepositAccount> TermDepositAccounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<GuardianRelationship> GuardianRelationships { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure default schema
            modelBuilder.HasDefaultSchema("training");
            
            // Configure inheritance
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<NormalUser>("NormalUser")
                .HasValue<ViewOnlyUser>("ViewOnlyUser")
                .HasValue<Admin>("Admin")
                .HasValue<SYSAdmin>("SYSAdmin");
            
            modelBuilder.Entity<Account>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<SavingAccount>("SavingAccount")
                .HasValue<CurrentAccount>("CurrentAccount")
                .HasValue<TermDepositAccount>("TermDepositAccount");
            
            // Configure cascade behavior to avoid cycles
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Bank)
                .WithMany()
                .HasForeignKey(ur => ur.BankId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Account)
                .WithMany()
                .HasForeignKey(ur => ur.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<GuardianRelationship>()
                .HasOne(gr => gr.Guardian)
                .WithMany(u => u.AsGuardian)
                .HasForeignKey(gr => gr.GuardianUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<GuardianRelationship>()
                .HasOne(gr => gr.Minor)
                .WithMany(u => u.AsMinor)
                .HasForeignKey(gr => gr.MinorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
