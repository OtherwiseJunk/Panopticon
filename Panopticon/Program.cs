using Discord.WebSocket;
using Discord;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Panopticon.Controllers;
using Panopticon.Data.Contexts;
using Panopticon.Data.Interfaces;
using Panopticon.Data.Services;
using Panopticon.Enums;
using Panopticon.Middleware;
using Panopticon.Services;
using Panopticon.Shared.Models.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DiscordSocketClient _socketClient;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = "https://dev-apsgkx34.us.auth0.com/";
    options.Audience = "https://panopticon.cacheblasters.com";

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = (context) =>
        {
            string? token = context.Request.Cookies["X-Access-Token"];
            if (token is not null)
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };

});

_socketClient = new(new DiscordSocketConfig { AlwaysDownloadUsers = true, GatewayIntents = GatewayIntents.All });

_socketClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DEEPSTATE")!).Wait();
_socketClient.StartAsync().Wait();
while (_socketClient.ConnectionState != ConnectionState.Connected) { Console.WriteLine("Waiting for discord client to be ready... Sleeping 1000ms"); Thread.Sleep(1000); }

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FeedbackService>()
                .AddSingleton<UserRecordService>()
                .AddSingleton<IOocService, OocService>()
                .AddSingleton<FFMPEGService>()
                .AddSingleton<DiscordService>()
                .AddSingleton<TokenService>()
                .AddSingleton<IApiKeyService, ApiKeyService>()
                .AddSingleton<ILibcoinService, LibcoinService>()
                .AddSingleton(
                    new DOSpacesService(
                        Environment.GetEnvironmentVariable("DOPUBLIC"),
                        Environment.GetEnvironmentVariable("DOSECRET"),
                        Environment.GetEnvironmentVariable("DOURL"),
                        Environment.GetEnvironmentVariable("DOBUCKET")
                    )
                )
                .AddSingleton(_socketClient);
builder.Services.AddLogging();
builder.Services.AddDbContextFactory<PanopticonContext>();

builder.Services.AddHttpClient<FREDService>();
builder.Services.AddHttpClient<PalantirAuthorizationController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseWhen(context => context.Request.Path.StartsWithSegments("/libcoin"), 
    builder => builder.UseMiddleware<ApiKeyMiddleware>());

app.MapControllers();

app.Run();

/*options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("*"));
}*/

void CreateNewApiKey(int permissions, string developerName, string keyPurpose)
{
    var apiServer = app.Services.GetService<IApiKeyService>();
    apiServer!.CreateApiKey(new ApiKey
    {
        Key = Guid.NewGuid().ToString(),
        Permissions = permissions,
        DeveloperName = developerName,
        KeyPurpose = keyPurpose,
        DateCreated = DateTime.Now
    });
}