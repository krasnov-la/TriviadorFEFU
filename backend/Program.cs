using DataAccess;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Services;
using Microsoft.AspNetCore.SignalR;
using Game;
using Utils;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

StaticDetails.DbConnection = Environment.GetEnvironmentVariable("DbConnect");
StaticDetails.JwtKey = Environment.GetEnvironmentVariable("Jwt");

if (StaticDetails.DbConnection is null)
{
    Secret? secret = JsonSerializer.Deserialize<Secret?>(File.ReadAllText("secret.json"));
    StaticDetails.DbConnection = secret?.Database;
    StaticDetails.JwtKey = secret?.JwtKey;
}

const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:9000");
                          policy.WithHeaders("x-requested-with", "x-signalr-user-agent", "authorization", "content-type");
                          policy.WithMethods("GET", "POST");
                          policy.AllowCredentials();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(StaticDetails.DbConnection));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserIdProvider, IdProvider>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.Audience = "https://localhost:7021";
    options.Authority = "https://localhost:7021";
    options.Configuration = new OpenIdConnectConfiguration();
    options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              var accessToken = context.Request.Query["access_token"];

              // If the request is for our hub...
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) &&
                  path.StartsWithSegments("/Game"))
              {
                  // Read the token out of the query string
                  context.Token = accessToken;
              }
              return Task.CompletedTask;
          }
      };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "fefudor/api",
        ValidAudience = "fefudor/client",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticDetails.JwtKey))
    };
});

builder.Services.AddSignalR();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/Game");

app.Run();