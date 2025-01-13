using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using PandaKidsServer.Common;
using PandaKidsServer.Handlers;
using PandaKidsServer.Messages;
using PandaKidsServer.OnlineUser;
using PandaKidsServer.ResManager;
using Serilog;
using AppContext = PandaKidsServer.AppContext;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/app.log",
        rollingInterval: RollingInterval.Day,
        shared: true
    ).CreateLogger();

var appContext = new AppContext();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:" + appContext.GetSettings().ListenPort);
builder.Services.AddControllers();
builder.Services.AddSingleton(appContext);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddMediatR(typeof(AppContext.Timer5SHandler).Assembly);
builder.Services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssemblies(typeof(AppContext.Timer5SHandler).GetTypeInfo().Assembly);
    }
);


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1 * 1024 * 1024 * 1024;
    options.AllowSynchronousIO = true;
});

var app = builder.Build();

var mediator = app.Services.GetRequiredService<IMediator>();
appContext.Init(mediator);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(Path.Combine(appContext.GetSettings().ResPath, "Resources")),
    RequestPath = "/Resources"
});

// app.UseDirectoryBrowser(new DirectoryBrowserOptions
// {
//     FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
//     RequestPath = "/Resources"
// });

OuterImgMiddleware.RootPath = appContext.GetResManager().GetBasePath();
app.UseOutImg();

app.Use(async (context, next) => {
    if (context.Request.Path == "/ws") {
        string? id = context.Request.Query["id"];
        if (Common.IsEmpty(id)) return;
        Console.WriteLine("ID: " + id);

        if (context.WebSockets.IsWebSocketRequest) {
            var websocket = await context.WebSockets.AcceptWebSocketAsync();
            var handler = new WebSocketHandler(appContext);
            var user = OnlineUser.Make(appContext, handler, id!);
            var userMgr = appContext.GetOnlineUserManager();
            userMgr.AddUser(user);
            await handler.Handle(websocket, id!);
        }
        else {
            context.Response.StatusCode = 400;
        }
    }
    else {
        await next();
    }
});

Log.Information("----> RUN <----");
app.Run();