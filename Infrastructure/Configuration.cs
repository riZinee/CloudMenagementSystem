using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.BackgroundTasks;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Configuration
    {
        public static IServiceCollection InfrastructureConfiguration
            (
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.AddHostedService<BackgroundTaskService>();

            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IIdentityService, IdentityService>();
            serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            serviceCollection.AddScoped<IFolderRepository, FolderRepository>();
            //serviceCollection.AddSingleton<ISftpService, SftpService>();
            serviceCollection.AddSingleton<IStorageService, LocalFileService>();
            serviceCollection.AddSingleton<IUploadProgressNotifier, UploadProgressNotifier>();
            serviceCollection.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            return serviceCollection;
        }
    }
}
