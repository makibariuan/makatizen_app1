using Microsoft.EntityFrameworkCore;
using makatizen_app.Server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using makatizen_app.Server.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Database Configuration ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
// ------------------------------

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "makatizen_app.Server", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below.",
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
            new string[] {}
        }
    });
});
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This setting tells the serializer to ignore any objects 
        // that create a circular reference path.
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// --- START JWT CONFIGURATION ---
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["Key"] ?? throw new ArgumentNullException("JWT Key not found in configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

//UserType: 1 = Super Admin, 2 = System User, 3 = Kit User
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireSuperAdmin", policy => policy.RequireClaim("UserType", "1"));
    options.AddPolicy("RequireSystemUser", policy => policy.RequireClaim("UserType", "2"));
    options.AddPolicy("RequireSuperAdminOrSystemUser", policy => policy.RequireClaim("UserType", "1","2"));
    options.AddPolicy("RequireKitUser", policy => policy.RequireClaim("UserType", "3"));
    options.AddPolicy("RequireAdminOrKit", policy => policy.RequireClaim("UserType", "1", "2", "3"));
});

// -----------------------------------------------------------------------------------------


// Add CORS policy for Vue.js frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueAppPolicy",
        policy =>
        {
            // Update origins to match your Vue.js dev server URL
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:8080",
                "https://localhost:58217",
                "https://localhost:7132"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.WebHost.UseUrls("http://0.0.0.0:5000"); // listen on all interfaces
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("VueAppPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();