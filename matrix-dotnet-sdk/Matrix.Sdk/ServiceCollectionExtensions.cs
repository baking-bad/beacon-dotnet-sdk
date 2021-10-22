namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using Core.Application;
    using Core.Application.Network;
    using Core.Application.Notifier;
    using Core.Domain;
    using Core.Domain.Room;
    using Core.Infrastructure.Repositories;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(MatrixApiConstants.Matrix,
                c => { c.BaseAddress = new Uri(MatrixApiConstants.BaseAddress); });
            // services.AddHttpClient();

            services.AddSingleton<MatrixCryptographyService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<MatrixServerService>();

            services.AddTransient<MatrixClient>();
            services.AddTransient<INetworkService, MatrixClientNetworkService>();

            services.AddTransient<ClientStateManager>();
            services.AddTransient<MatrixRoomFactory>();
            services.AddTransient<MatrixEventNotifier<List<BaseRoomEvent>>>();

            services.AddSingleton<ISeedRepository, MemorySeedRepository>();

            return services;
        }
    }
}