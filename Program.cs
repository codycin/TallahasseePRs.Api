using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.Data.Configurations;
using TallahasseePRs.Api.Models;
using TallahasseePRs.Api.Models.Users;
using TallahasseePRs.Api.Security;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.FeedServices;
using TallahasseePRs.Api.Services.FollowServices;
using TallahasseePRs.Api.Services.Media;
using TallahasseePRs.Api.Services.Notifications;
using TallahasseePRs.Api.Services.PostServices;
using TallahasseePRs.Api.Services.ProfileServices;
using TallahasseePRs.Api.Services.Storage;
using static TallahasseePRs.Api.Services.CurrentUserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Swagger Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Auth/User Services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<IProfileService>(sp => sp.GetRequiredService<ProfileService>());
builder.Services.AddScoped<IProfileQueryService>(sp => sp.GetRequiredService<ProfileService>());

// Post services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IVoteService, VoteService>();

// Feed service
builder.Services.AddScoped<IFeedService, FeedService>();

// Notifications
builder.Services.AddScoped<INotificationService, NotificationService>();

// Media service
builder.Services.Configure<MediaOptions>(
    builder.Configuration.GetSection("Media"));
builder.Services.AddScoped<IMediaService, MediaService>();

// Storage services
builder.Services.Configure<R2Options>(
    builder.Configuration.GetSection("R2"));

var storageProvider = builder.Configuration["Storage:Provider"];

if (string.Equals(storageProvider, "Local", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IObjectStorage, LocalStorageService>();
}
else
{
    builder.Services.AddScoped<IObjectStorage, CloudflareR2StorageService>();
}

// JWT config from appsettings json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("Jwt:Key is missing from configuration.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            RoleClaimType = ClaimTypes.Role,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();

    if (!db.Lifts.Any())
    {
        db.Lifts.Add(new Lift
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Bench Press"
        });

        db.SaveChanges();
    }

    await AdminSeeder.SeedAsync(db);
}

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "LocalUploads");
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();