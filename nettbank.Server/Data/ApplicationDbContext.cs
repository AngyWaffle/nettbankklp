using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationDbContext : DbContext
{
    // Constructor
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Define the tables
    public DbSet<Users> Users { get; set; }
    public DbSet<Accounts> Accounts { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<AccountBalance> AccountBalance { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define unique indexes
        modelBuilder.Entity<Accounts>()
        .HasIndex(a => a.AccountNumber)
        .IsUnique();

        // Define primary keys and relationships
        modelBuilder.Entity<Accounts>()
            .HasKey(a => a.AccountNumber);

        modelBuilder.Entity<Accounts>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transactions>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Transactions>()
            .HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountNumber)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AccountBalance>()
            .HasKey(ab => ab.Id);

        modelBuilder.Entity<AccountBalance>()
            .HasOne(ab => ab.Account)
            .WithMany()
            .HasForeignKey(ab => ab.AccountNumber)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


//User table
public class Users
{
    [Key]
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Password { get; set; }
    public string Mail { get; set; }
    public string Salt { get; set; }
}

//Accounts table
public class Accounts
{
    [Key]
    public long AccountNumber { get; set; }  // Primary Key

    public int UserId { get; set; }
    public string AccountType { get; set; }

    // Add a Foreign Key reference to Users table
    [ForeignKey("UserId")]
    public Users User { get; set; }
}


//Transaction table for transaction history
public class Transactions
{
    [Key]
    public int Id { get; set; }  // Primary Key

    public long AccountNumber { get; set; }
    public long AccountNumberReceived { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }

    // Add Foreign Key to Accounts
    [ForeignKey("AccountNumber")]
    public Accounts Account { get; set; }
}


//Account balanance table for account balance
public class AccountBalance
{
    [Key]
    public int Id { get; set; }

    public long AccountNumber { get; set; }
    public decimal Balance { get; set; }

    // Add Foreign Key to Accounts
    [ForeignKey("AccountNumber")]
    public Accounts Account { get; set; }
}
