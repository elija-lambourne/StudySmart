using Microsoft.EntityFrameworkCore;
using StudySmortAPI.Model;

namespace StudySmortAPI;

public class DataContext : DbContext
{
    private readonly string _dbPath;
    public DbSet<Deadline> Deadlines { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<Notebook> Notebooks { get; set; }
    public DbSet<User> Users { get; set; }

    public DataContext()
    {
        var rootDir = Directory.GetCurrentDirectory();
        var databaseName = Path.Combine(rootDir, "Data/db.sqlite3");
        _dbPath = databaseName;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Flashcards)
            .WithOne(f => f.Owner)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Folders)
            .WithOne(f => f.Owner)
            .HasForeignKey(f => f.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Deadlines)
            .WithOne(d => d.Owner)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notebook>()
            .HasOne(n => n.Owner)
            .WithMany()
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Folder>()
            .HasMany(f => f.ChildFolders)
            .WithOne()
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Folder>()
            .HasMany(f => f.ChildNotebooks)
            .WithOne()
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Notebook>()
            .HasKey(f => f.Id);

        // Add any additional entity configurations or relationships here

        base.OnModelCreating(modelBuilder);
    }
}