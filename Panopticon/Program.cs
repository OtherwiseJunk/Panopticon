using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                .AddSingleton<OOCService>();

builder.Services.AddDbContextFactory<PanopticonContext>();

builder.Services.AddHttpClient<FREDService>();


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
app.UseCors(builder => builder.WithOrigins("*"));

app.MapControllers();

app.Run();

/*options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("*"));
}*/