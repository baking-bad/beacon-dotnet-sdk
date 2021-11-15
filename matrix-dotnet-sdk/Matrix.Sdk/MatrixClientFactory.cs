namespace Matrix.Sdk
{
    using System.Net.Http;
    using Core.Domain.Services;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.Logging;

    public class SingletonHttpFactory : IHttpClientFactory
    {
        private readonly HttpClient _httpClient = new();
        
        public HttpClient CreateClient(string name) => _httpClient;
    } 
    
    
    public class MatrixClientFactory
    {
        private readonly SingletonHttpFactory _httpClientFactory = new();
        private MatrixClient? _client;

        public IMatrixClient Create(ILogger<PollingService>? logger = null)
        {
            if (_client != null)
                return _client;

            var eventService = new EventService(_httpClientFactory);
            var networkService = new NetworkService
            (
                eventService,
                new RoomService(_httpClientFactory), 
                new UserService(_httpClientFactory)
            );

            var pollingService = new PollingService(eventService, logger);

            _client = new MatrixClient(networkService, pollingService);

            return _client;
        }
    }
}