namespace MatrixSdk
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMatrixSdk(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<UserService>();

            return services;
        }
    }
}