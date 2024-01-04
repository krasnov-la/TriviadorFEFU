using DataAccess;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secret.json", false);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins", policy => { policy.WithOrigins("*"); policy.WithHeaders("*"); });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetSection("connectionStrings").GetSection("default").Value));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7021",
        ValidAudience = "https://localhost:7021",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtKey").Value))
    };
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var questions = context.Questions.ToList();
    var answerOptions = context.Answers.ToList();

    // Вывести данные в консоль или логгер
    Console.WriteLine("Questions:");
    foreach (var question in questions)
    {
        Console.WriteLine($"Question Id: {question.Id}, Text: {question.Text}");
        foreach (var option in question.Options)
        {
            Console.WriteLine($"  Option Id: {option.Id}, Text: {option.Text}, Correct: {option.Correct}");
        }
    }

    Console.WriteLine("AnswerOptions:");
    foreach (var option in answerOptions)
    {
        Console.WriteLine($"Option Id: {option.Id}, Text: {option.Text}, Correct: {option.Correct}, QuestionId: {option.QuestionId}");
    }
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
