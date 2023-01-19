using EscapeSandBox.Api.Core;
using EscapeSandBox.Api.Domain;
using EscapeSandBox.Api.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace EscapeSandBox.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration
#if !DEBUG
                .AddJsonFile("host.json")
#endif
                .AddJsonFile("appsettings.json");

            new ApiConfig().Initialize(builder.Configuration);

            builder.Services.AddLogging(log4net => { log4net.AddLog4Net("log4net.config"); });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(ApiConfig.DefaultCors, policy =>
                {
                    policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = ApiConfig.Issuer,
                    ValidAudience = ApiConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiConfig.IssuerSigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            }); ;

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy.
                // options.FallbackPolicy = options.DefaultPolicy;
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim(ClaimTypes.Email, "minxing.bao@wimisoft.com"));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();

            builder.Services.AddTransient(typeof(IDapperRepository<,>), typeof(DapperRepository<,>));

            builder.Services.AddSingleton<ITokenHelper, TokenHelper>();

            builder.Services.AddSingleton<INginxManager, NginxManager>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(ApiConfig.DefaultCors);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }

    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);

            if (dateOfBirthClaim is null)
            {
                return Task.CompletedTask;
            }

            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
            int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;

        public int MinimumAge { get; }
    }
}