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

    // TODO: set a better window title
    .SetWindowTitle("Block-Break")

    // TODO: add any resources needed (refer to PlayPlayMini documentation for more info)
    .AddAssets(new IAsset[]
    {
        new SpriteSheetMeta("Blocks", "Graphics/Blocks", 24, 8),
        new PictureMeta("Ball", "Graphics/Ball", true),
        new PictureMeta("Life", "Graphics/Life"),
        new PictureMeta("Paddle", "Graphics/Paddle"),
        new FontMeta("Font", "Fonts/Font", 6, 8, true),
        new PictureMeta("GameOver", "Graphics/GameOver")
    })

    // TODO: any additional service registration (refer to PlayPlayMini and/or Autofac documentation for more info)
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