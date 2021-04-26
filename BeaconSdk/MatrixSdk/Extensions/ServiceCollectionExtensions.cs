namespace MatrixSdk.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Providers;
    using Repositories;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(Constants.Matrix, c => { c.BaseAddress = new Uri("https://matrix.papers.tech/"); });

            services.AddSingleton<MatrixCryptoService>();
            services.AddSingleton<MatrixEventService>();
            services.AddSingleton<MatrixRoomService>();
            services.AddSingleton<MatrixUserService>();

            services.AddSingleton<AccessTokenProvider>();
            services.AddSingleton<ICryptoAlgorithmsProvider, LibsodiumAlgorithmsProvider>();
            
            services.AddSingleton<ISeedRepository, InMemorySeedRepository>();

            return services;
        }
    }
}