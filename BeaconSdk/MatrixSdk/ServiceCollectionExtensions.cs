namespace MatrixSdk
{
    using System;
    using Application;
    using Infrastructure;
    using Infrastructure.Providers;
    using Infrastructure.Repositories;
    using Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(MatrixApiConstants.Matrix, c => { c.BaseAddress = new Uri(MatrixApiConstants.BaseAddress); });

            services.AddSingleton<CryptoService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();

            services.AddTransient<MatrixClient>();
            services.AddTransient<ClientStateManager>();
            services.AddTransient<MatrixRoomFactory>();
            services.AddTransient<TextMessageNotifier>();

            services.AddSingleton<AccessTokenProvider>();
            services.AddSingleton<ICryptoAlgorithmsProvider, LibsodiumAlgorithmsProvider>();

            services.AddSingleton<ISeedRepository, MemorySeedRepository>();

            return services;
        }
    }
}