using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManagmentProject.Entities
{
    public class PersonalFinanceManagmentDbContext : DbContext
    {
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        public PersonalFinanceManagmentDbContext()
        {

        }

        public PersonalFinanceManagmentDbContext(DbContextOptions<PersonalFinanceManagmentDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bill configuration
            modelBuilder.Entity<Bill>(eb =>
            {
                eb.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
                eb.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(14, 2);
                eb.HasMany(b => b.Transactions)
                .WithOne(t => t.Bill)
                .HasForeignKey(t => t.BillId);
            });

            // Category configuration
            modelBuilder.Entity<Category>(eb =>
            {
                eb.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(20);
                eb.Property(p => p.Color)
                .IsRequired()
                .HasMaxLength(20);
                eb.Property(p => p.Icon)
                .IsRequired()
                .HasMaxLength(20);
                eb.HasMany(c => c.Transactions)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.CategoryId);
            });

            // Transaction configuration
            modelBuilder.Entity<Transaction>(eb =>
            {
                eb.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
                eb.Property(p => p.Status)
                .IsRequired();
                eb.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(14, 2);
            });
        }
    }
}
