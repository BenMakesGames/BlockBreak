using BenMakesGames.PlayPlayMini;
using BenMakesGames.PlayPlayMini.Model;
using BlockBreak.GameStates;
using BlockBreak.Services;
using Serilog;
using Serilog.Extensions.Autofac.DependencyInjection;

FileSystemHelpers.EnsureDirectoryExists();

var gsmBuilder = new GameStateManagerBuilder();

gsmBuilder
    .SetWindowSize(1920 / 5, 1080 / 5, 3)
    .SetInitialGameState<Startup>()

    .SetWindowTitle("Block-Break")

    .AddAssets(new IAsset[]
    {
        new FontMeta("Font", "Fonts/Font", 6, 8, true),

        new SpriteSheetMeta("Blocks", "Graphics/Blocks", 24, 8),
        new SpriteSheetMeta("BigWords", "Graphics/BigWords", 201, 27),

        new PictureMeta("Ball", "Graphics/Ball", true),
        new PictureMeta("Life", "Graphics/Life"),
        new PictureMeta("Paddle", "Graphics/Paddle"),
        new PictureMeta("MenuCursor", "Graphics/MenuCursor"),
        
        new SoundEffectMeta("Bounce", "Sounds/Bounce"),
    })

    .AddServices(s => {
        var serilogConfig = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
            .WriteTo.Console()
#else
            .MinimumLevel.Warning()
#endif
            .WriteTo.File($"{FileSystemHelpers.GameDataPath}{Path.DirectorySeparatorChar}Log.log", rollingInterval: RollingInterval.Day)
        ;

        s.RegisterSerilog(serilogConfig);
    })
;

gsmBuilder.Run();