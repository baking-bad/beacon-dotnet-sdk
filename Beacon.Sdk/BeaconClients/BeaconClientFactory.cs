namespace Beacon.Sdk.BeaconClients
{
    using Abstract;
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
            
            ServiceProvider? beaconServicesProvider = beaconServices.BuildServiceProvider();
            return beaconServicesProvider.GetRequiredService<T>();
        }
    }
}