namespace MatrixSdk
{
    using System;
    using System.Collections.Generic;
    using Application;
    using Application.Network;
    using Application.Notifier;
    using Domain;
    using Domain.Room;
    using Infrastructure;
    using Infrastructure.Repositories;
    using Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(MatrixApiConstants.Matrix, c => { c.BaseAddress = new Uri(MatrixApiConstants.BaseAddress); });
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