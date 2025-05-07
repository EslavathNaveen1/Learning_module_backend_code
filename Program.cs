using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QtechBackend.Context;
using QtechBackend.Repositories;
using QtechBackend.RepositoryInterface;
using QtechBackend.ServiceInterface;
using QtechBackend.Services;
using QtechBackend.Interfaces;
using SendingOTP.Container;
using SendingOTP.Model;
using SendingOTP.services;
using SendingOTP.Controllers;
using QtechBackendv2.RepositoryInterface;
using QtechBackendv2.ServiceInterface;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;

namespace qtechbackend
{
    public class program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddAutoMapper(typeof(program));

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB limit
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Qtech API",
                    Version = "v1",
                    Description = "API for Qtech Backend Services"
                });

                c.EnableAnnotations();
                c.OperationFilter<FileUploadOperationFilter>();
            });
            builder.Services.AddScoped<userService>();
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailservice, EmailService>();
            // Configure the database context
            builder.Services.AddDbContext<ElearningContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultconnection")));

            // Configure JWT authentication
            var jwtsettings = builder.Configuration.GetSection("jwt");
            var key = Encoding.ASCII.GetBytes(jwtsettings["key"]);

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
                    ValidIssuer = jwtsettings["issuer"],
                    ValidAudience = jwtsettings["audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // UPDATED: Add CORS services with specific configuration
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins("https://agreeable-stone-0a2163e1e.6.azurestaticapps.net")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials(); // Important for authentication
                });
            });

            // Register services and repositories
            builder.Services.AddScoped<IVideoService, VideoService>();
            builder.Services.AddScoped<IVideoRepository, VideoRepository>();
            builder.Services.AddScoped<IDocumentationService, DocumentationService>();
            builder.Services.AddScoped<IDocumentationRepository, DocumentationRepository>();
            builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<OtpController>();
            builder.Services.AddScoped<IEnrolledRepository, EnrolledRepository>();
            builder.Services.AddScoped<IEnrolledService, EnrolledService>();

            // Register OtpController and its dependencies
            builder.Services.AddScoped<OtpController>();
            builder.Services.AddScoped<IEmailservice, EmailService>(); // Example dependency

            // Configure authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDF API V1");
                });
            }

            app.UseHttpsRedirection();

            // IMPORTANT: Use CORS before authentication and authorization
            app.UseCors();

            // Add middleware to handle OPTIONS requests explicitly
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "https://agreeable-stone-0a2163e1e.6.azurestaticapps.net");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    context.Response.StatusCode = 200;
                    return;
                }

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

