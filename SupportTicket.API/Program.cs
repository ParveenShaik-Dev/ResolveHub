using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SupportTicket.API.Data;
using SupportTicket.API.Helpers;
using SupportTicket.API.Repositories;
using SupportTicket.API.Repositories.Interfaces;
using SupportTicket.API.Services;
using SupportTicket.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//FIX 1: Build correct SQLite path for Azure AND local
var homeDir = Environment.GetEnvironmentVariable("HOME");
string dbPath;

if (!string.IsNullOrEmpty(homeDir))
{
    // Running on Azure App Service → write to C:\home\data\
    var dataDir = Path.Combine(homeDir, "data");
    Directory.CreateDirectory(dataDir); // Create folder if it doesn't exist
    dbPath = Path.Combine(dataDir, "ticketdb.sqlite");
}
else
{
    // Running locally then use project folder (same as before)
    dbPath = "ticketdb.sqlite";
}

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TicketAPI", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token here}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//FIX 2: Use the dynamic dbPath instead of hardcoded relative path
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors();

var app = builder.Build();

//FIX 3: Run DB migration BEFORE SeedAdminUser
// This creates the SQLite file + all tables automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//FIX 4: Show Swagger on Azure too (so you can test the API)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//  This now runs AFTER migration, so DB plus tables exist
app.SeedAdminUser();

app.Run();