namespace MatrixSdk.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Newtonsoft.Json;

    public class UnstableFeatures
    {
        [JsonProperty("org.matrix.label_based_filtering")]
        public bool OrgMatrixLabelBasedFiltering { get; set; }

        [JsonProperty("org.matrix.e2e_cross_signing")]
        public bool OrgMatrixE2eCrossSigning { get; set; }

        [JsonProperty("org.matrix.msc2432")] public bool OrgMatrixMsc2432 { get; set; }

        [JsonProperty("uk.half-shot.msc2666")] public bool UkHalfShotMsc2666 { get; set; }

        [JsonProperty("io.element.e2ee_forced.public")]
        public bool IoElementE2eeForcedPublic { get; set; }

        [JsonProperty("io.element.e2ee_forced.private")]
        public bool IoElementE2eeForcedPrivate { get; set; }

        [JsonProperty("io.element.e2ee_forced.trusted_private")]
        public bool IoElementE2eeForcedTrustedPrivate { get; set; }

        [JsonProperty("org.matrix.msc3026.busy_presence")]
        public bool OrgMatrixMsc3026BusyPresence { get; set; }
    }

    public class MatrixServerVersionsResponse
    {
        public List<string> versions { get; set; }
        public UnstableFeatures unstable_features { get; set; }
    }

    public class MatrixServerService
    {
        private const string RequestUri = "_matrix/client/versions";
        private readonly IHttpClientFactory _httpClientFactory;

        public MatrixServerService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MatrixServerVersionsResponse> GetMatrixClientVersions(Uri baseAddress, CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = baseAddress;

            return await httpClient.GetAsJsonAsync<MatrixServerVersionsResponse>($"{RequestUri}", cancellationToken);
        }
    }
}