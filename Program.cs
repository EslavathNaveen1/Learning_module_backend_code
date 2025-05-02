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

      
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();
            builder.Services.AddAutoMapper(typeof(program));

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
            });
     
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
           
            builder.Services.AddDbContext<ElearningContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("defaultconnection")));

          
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

            
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

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

            
            builder.Services.AddScoped<OtpController>();
            builder.Services.AddScoped<IEmailservice, EmailService>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDF API V1");
                });
            }


            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}




