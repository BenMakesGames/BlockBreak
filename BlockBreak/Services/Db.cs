using BenMakesGames.PlayPlayMini.Attributes.DI;
using BlockBreak.Model.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace BlockBreak.Services;

[AutoRegister(Lifetime.PerDependency)]
public sealed class Db: DbContext
{
    public DbSet<HighScoreEntry> HighScoreEntries => Set<HighScoreEntry>();
    public DbSet<Settings> Settings => Set<Settings>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var filePath = $"{FileSystemHelpers.GameDataPath}{Path.DirectorySeparatorChar}data.db";
        optionsBuilder.UseSqlite($"Data Source={filePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HighScoreEntry>().HasData(
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b0"), Name = "OMG", Score = 161250 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b1"), Name = "WOW", Score = 146300 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b2"), Name = "BEN", Score = 132500 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b3"), Name = "ENM", Score = 117750 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b4"), Name = "VER", Score = 103500 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b5"), Name = "ROY", Score = 89250 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b6"), Name = "OFT", Score = 75000 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b7"), Name = "BBQ", Score = 60750 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b8"), Name = "LOL", Score = 46500 },
            new() { Id = Guid.Parse("2d75a0f4-bac1-41b2-811d-20e7d6ee96b9"), Name = "AFK", Score = 32250 }
        );
    }
}