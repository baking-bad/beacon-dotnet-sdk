namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using Clients;
    using Core;
    using Core.Domain;
    using Core.Domain.Room;
    using Core.Infrastructure.Repositories;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Extensions methods to configure an <see cref="IServiceCollection" /> for <see cref="IHttpClientFactory" /> with
    ///     Matrix Sdk.
    /// </summary>
    public static class MatrixClientServiceExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(Constants.Matrix,
                c => { c.BaseAddress = new Uri(Constants.BaseAddress); });
            // services.AddHttpClient();

            services.AddSingleton<MatrixCryptographyService>();
            services.AddSingleton<EventClient>();
            services.AddSingleton<RoomClient>();
            services.AddSingleton<UserClient>();
            services.AddSingleton<MatrixServerService>();
            services.AddTransient<MatrixClient>();

            services.AddTransient<MatrixClientStateManager>();
            services.AddTransient<MatrixRoomFactory>();
            services.AddTransient<MatrixEventNotifier<List<BaseRoomEvent>>>();

            services.AddSingleton<ISeedRepository, MemorySeedRepository>();

            return services;
        }
    }
}