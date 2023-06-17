using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudySmortAPI;

var builder = WebApplication.CreateBuilder(args);
const string policy = "AllowOrigin";
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(c =>
{
    c.AddPolicy(policy, options =>
    {
        options.AllowAnyOrigin();
        options.AllowAnyHeader();
        options.AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
           ValidIssuer = config["JwtSettings:Issuer"],
           ValidAudience = config["JwtSettings:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Key"] ?? "sjkhwlakejh2kljh23kjh4kjndsakjfkjh43kjh")),
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
}

app.UseCors(policy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();