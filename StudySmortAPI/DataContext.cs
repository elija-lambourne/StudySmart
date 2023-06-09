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
    public DbSet<FlashcardCategory> FlashcardCategories { get; set; }

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
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            // Define relationships
            entity.HasMany(u => u.FlashcardCategories)
                .WithOne(c => c.Owner)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Deadlines)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(u => u.RootDir)
                .WithOne()
                .HasForeignKey<User>(u => u.FolderId);
            
            // Additional configurations for User entity...
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.Password).IsRequired();
        });

        modelBuilder.Entity<Notebook>(entity =>
        {
            entity.HasKey(n => n.Id);

            // Define relationships
            entity
                .HasOne(n => n.Owner)
                .WithMany()
                .HasForeignKey(n => n.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity
                .HasOne(n => n.Parent)
                .WithMany(f => f.ChildNotebooks)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            // Additional configurations for Notebook entity...
        });

        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(f => f.FolderId);
            // Define relationships

            entity.HasMany(f => f.ChildNotebooks)
                .WithOne(x => x.Parent)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(f => f.ChildFolders)
                .WithOne(cf => cf.ParentFolder)
                .HasForeignKey(cf => cf.ParentFolderId)
                .OnDelete(DeleteBehavior.Cascade);
        
            // Additional configurations for Folder entity...
        });

        modelBuilder.Entity<FlashcardCategory>(entity =>
        {
            entity.HasKey(c => c.Id);

            // Define relationships
            entity.HasOne(c => c.Owner)
                .WithMany(u => u.FlashcardCategories)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Flashcards)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);


            // Additional configurations for FlashcardCategory entity...
        });

        modelBuilder.Entity<Deadline>(entity =>
        {
            entity.HasKey(d => d.DeadlineId);

            // Define relationships
            entity.HasOne(d => d.Owner)
                .WithMany(u => u.Deadlines)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Additional configurations for Deadline entity...
        });

        modelBuilder.Entity<Flashcard>()
            .HasOne(f => f.Category)
            .WithMany(c => c.Flashcards)
            .HasForeignKey(f => f.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        
        base.OnModelCreating(modelBuilder);
    }
}