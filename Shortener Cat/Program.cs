using Core.Domain.Entities;
using Core.Domain.RepositoryContracts;
using Core.Services;
using Core.ServicesContracts;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.DB;
using Infrastructure.Externals;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Extensions.Logging;
using Shortener_Cat.Filters;
using Shortener_Cat.Middlewares;
using System.Text;

namespace Shortener_Cat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Redis
            builder.Services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = builder.Configuration.GetConnectionString("Redis");
            });

            // Serilog
            Log.Logger = new LoggerConfiguration().WriteTo.File("Logs/log.txt").WriteTo.Console().CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger));

            // DB
            string dbConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(dbConnection);
            });
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Authentication & Authorization
            var jwtConfig = builder.Configuration.GetSection("JWT");
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidAudience = jwtConfig["Aud"],
                    ValidIssuer = jwtConfig["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
                };
            });

            // API versioning
            builder.Services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new UrlSegmentApiVersionReader();
                opt.DefaultApiVersion = new(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
            });

            // Swagger
            //builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
            builder.Services.AddSwaggerGen(opts =>
            {
                string[] versions = ["v1"];

                foreach (string version in versions)
                {
                    opts.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Shortener Cat API Documentation",
                        Description = "API for Shortener Cat, a URL shortener app and website.",
                        Version = version
                    });
                }

                opts.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

                opts.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Context accessor
            builder.Services.AddHttpContextAccessor();

            // Device detection
            builder.Services.AddDetection();

            // Firebase 
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("shortenerfb.json")
            });

            // Custom
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IExpiredTokensRepo, ExpiredTokensRepo>();
            builder.Services.AddScoped<IShortUrlsRepo, ShortUrlsRepo>();
            builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            builder.Services.AddScoped<BlackListTokenFilter>();
            builder.Services.AddScoped<IEmailSender, MailTrapSender>();
            builder.Services.AddScoped<IShortenerService, ShortenerService>();
            builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
            builder.Services.AddScoped<IUrlVisitsRepo, UrlVisitsRepo>();
            builder.Services.AddScoped<IUrlVisitService, UrlVisitService>();
            builder.Services.AddScoped<IAnalyticsRepo, AnalyticsRepo>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IDeviceTokensRepo, DeviceTokensRepo>();
            builder.Services.AddScoped<IPushNotificationService, FCMNotificationService>();

            builder.Services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                opt.User.RequireUniqueEmail = true;
            });

            var app = builder.Build();

            // Auto migrate on startup for docker containers 
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            app.UseGlobalExceptionHandler();

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("v1/swagger.json", "1.0");
            });

            if (!builder.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
