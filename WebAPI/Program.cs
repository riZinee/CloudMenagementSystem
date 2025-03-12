
using Application;
using Application.Commands.CreateUser;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Configurations;
using Infrastructure.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Middlewares;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Rejestracja EF Core
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddCors();
            // Rejestracja MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommandHandler).Assembly));

            builder.Services.AddSignalR();

            builder.Services.ApplicationConfiguration(builder.Configuration);
            builder.Services.InfrastructureConfiguration(builder.Configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var jwtSection = builder.Configuration.GetSection("JwtData");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var secret = jwtSection["Secret"];

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer [jwt]'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
            });

            builder.Services.AddAuthorization();

            //Add Authentication JWT
            builder.Services
                .AddAuthentication(opt =>
                {
                    //Creating Default Scheme [We can use in different Controllers Differnt Scheme]
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true, //ClockSkew
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        ClockSkew = TimeSpan.Zero, //Allowed Expired Tokens,ex. TimeSpan.FromMinutes(1)
                        ValidIssuer = issuer, //Who Gives Token
                        ValidAudience = audience, //For Who Given Token
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), //Secret
                    };

                    //Returns info about Expired Token
                    opt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("Token-expired", "true");
                            }
                            return Task.CompletedTask;
                        }//,
                        //OnMessageReceived = context =>
                        //{
                        //    var accessToken = context.Request.Query["access_token"];

                        //    if (!string.IsNullOrEmpty(accessToken))
                        //    {
                        //        context.Token = accessToken;
                        //        Console.WriteLine($"Token przekazany przez QueryString: {accessToken}");
                        //    }
                        //    else
                        //    {
                        //        Console.WriteLine("Brak tokena w QueryString!");
                        //    }

                        //    return Task.CompletedTask;
                        //},
                        //OnTokenValidated = context =>
                        //{
                        //    Console.WriteLine($"Token prawid³owy: {context.SecurityToken}");
                        //    return Task.CompletedTask;
                        //}
                    };
                });



            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 10L * 1024 * 1024 * 1024; // 10GB dodaj do configu
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
                StoredProcedureConfiguration.EnsureStoredProceduresCreated(dbContext);
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request: {context.Request.Path}");
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    Console.WriteLine($"Token: {context.Request.Headers["Authorization"]}");
                }
                await next();
            });

            app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapHub<UploadProgressHub>("/uploadProgressHub");
            app.MapControllers();

            app.Run();
        }
    }
}
