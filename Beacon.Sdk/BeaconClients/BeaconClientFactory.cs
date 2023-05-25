namespace Beacon.Sdk.BeaconClients
{
    using Abstract;
    using Core.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class BeaconClientFactory
    {
        public static T Create<T>(
            BeaconOptions? options,
            ILoggerProvider? loggerProvider = null) where T : notnull
        {
            var beaconServices = new ServiceCollection();
            
            if (typeof(T) == typeof(IWalletBeaconClient))
                beaconServices.AddBeaconWalletClient(options, loggerProvider);
            
            if (typeof(T) == typeof(IDappBeaconClient))
                beaconServices.AddBeaconDappClient(options, loggerProvider);
            
            var beaconServicesProvider = beaconServices.BuildServiceProvider();
            var service = beaconServicesProvider.GetRequiredService<T>();

            if (service is IBaseBeaconClient client)
            {
                var connectionPool = beaconServicesProvider.GetRequiredService<ILiteDbConnectionPool>();

                if (connectionPool != null)
                {
                    client.OnDisconnected += () =>
                    {
                        connectionPool.CloseAllConnections(); // close all LiteDb connections after disconnected event
                    };
                }
            }

            return service;
        }
    }
}