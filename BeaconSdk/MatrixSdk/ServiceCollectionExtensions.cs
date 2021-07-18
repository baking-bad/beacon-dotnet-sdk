namespace MatrixSdk
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
            services.AddHttpClient(MatrixConstants.Matrix, c => { c.BaseAddress = new Uri(MatrixConstants.BaseAddress); });

            services.AddSingleton<CryptoService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();
            services.AddTransient<MatrixClient>();

            services.AddSingleton<AccessTokenProvider>();
            services.AddSingleton<ICryptoAlgorithmsProvider, LibsodiumAlgorithmsProvider>();

            services.AddSingleton<ISeedRepository, MemorySeedRepository>();

            return services;
        }
    }
}