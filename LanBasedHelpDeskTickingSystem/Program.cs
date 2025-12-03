using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Implementations;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using LanBasedHelpDeskTickingSystem.Services.Implementations;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.RateLimiting;
using GenerativeAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var configuration = builder.Configuration;
var apiKey = builder.Configuration["Gemini:ApiKey"] ?? "";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 39)) // set to your MySQL version
    )
);

// Add password hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<GoogleAi>(provider => new GoogleAi(apiKey));
builder.Services.AddScoped<IAiSupportService, AiSupportService>();

// Register repositories
builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IArchiveRepository, ArchiveRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<INotifyRepository, NotifyRepository>();

// MVC controllers + views
builder.Services.AddControllersWithViews();

// JWT Authentication
var key = configuration["Jwt:Key"] ?? "";
var issuer = configuration["Jwt:Issuer"] ?? "";
var audience = configuration["Jwt:Audience"] ?? "";

if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    throw new InvalidOperationException("JWT configuration is missing.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

// Add Anti-Forgery token validation globally
// builder.Services.AddControllersWithViews(options =>
// {
//     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
// });

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", config =>
    {
        config.PermitLimit = 5;            // allow 5 requests
        config.Window = TimeSpan.FromSeconds(10); // per 10 seconds
        config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 2;             // optional queue size
    });
});

// builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
//         // Lockout settings
//         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lock for 5 mins
//         options.Lockout.MaxFailedAccessAttempts = 5; // Max 5 tries
//         options.Lockout.AllowedForNewUsers = true; // Apply to new users immediately
//     })
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // for API calls
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme; // important for external login state
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // true for production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
            RoleClaimType = ClaimTypes.Role
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("jwt"))
                {
                    context.Token = context.Request.Cookies["jwt"];
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(IdentityConstants.ExternalScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddGoogle(options =>
    {
        var google = builder.Configuration.GetSection("Authentication:Google");
        
        options.ClientId = google["ClientId"] ?? "";
        options.ClientSecret = google["ClientSecret"] ?? "";
        
        options.Scope.Add("profile");
        options.Scope.Add("email");
        
        options.SignInScheme = IdentityConstants.ExternalScheme;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(); //  FIX 2

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();
app.MapControllers();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();