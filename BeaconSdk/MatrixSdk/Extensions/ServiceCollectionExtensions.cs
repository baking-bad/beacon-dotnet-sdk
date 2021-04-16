namespace MatrixSdk.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Providers;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient(Constants.Matrix, c => { c.BaseAddress = new Uri("https://matrix.papers.tech/"); });

            services.AddSingleton<MatrixUserService>();
            services.AddSingleton<MatrixRoomService>();

            services.AddSingleton<AccessTokenProvider>();
            services.AddSingleton<MatrixCryptoService>();
            services.AddSingleton<ICryptoAlgorithmsProvider, LibsodiumAlgorithmsProvider>();

            return services;
        }
    }
}