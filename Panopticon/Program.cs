using Microsoft.AspNetCore.Authentication.JwtBearer;
using Panopticon.Controllers;
using Panopticon.Data.Contexts;
using Panopticon.Data.Services;
using Panopticon.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = "https://dev-apsgkx34.us.auth0.com/";
    options.Audience = "https://panopticon.cacheblasters.com";
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FeedbackService>()
                .AddSingleton<UserRecordService>()
                .AddSingleton<OOCService>()
                .AddSingleton<FFMPEGService>()
                .AddSingleton(
                    new DOSpacesService(
                        Environment.GetEnvironmentVariable("DOPUBLIC"),
                        Environment.GetEnvironmentVariable("DOSECRET"),
                        Environment.GetEnvironmentVariable("DOURL"),
                        Environment.GetEnvironmentVariable("DOBUCKET")
                    )
                );
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
app.UseCors(builder =>
{
    builder.WithOrigins("*");
    builder.WithHeaders("Origin, X-Requested-With, Content-Type, Accept");
}
) ;

app.MapControllers();

app.Run();

/*options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("*"));
}*/