using BenMakesGames.PlayPlayMini.Attributes.DI;
using BlockBreak.Model.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace BlockBreak.Services;

[AutoRegister(Lifetime.PerDependency)]
public sealed class Db: DbContext
{
    public DbSet<HighScoreEntry> HighScoreEntries => Set<HighScoreEntry>();
}