namespace Beacon.Examples.ConsoleApp
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Base58Check;
    using Matrix.Sdk;
    using Matrix.Sdk.Listener;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Sdk.Core.Infrastructure.Cryptography;
    using Sdk.Core.Transport.P2P;
    using Sdk.Core.Transport.P2P.ChannelOpening;
    using Sdk.Core.Transport.P2P.Dto.Handshake;
    using Sodium;

    public record P2PClientOptions(string AppName, string RelayServer);
    
    public static class BeaconClientScenarios
    {
        public static async Task Setup(IServiceProvider serviceProvider)
        {
            var QRCode =
                "3NDKTWt2x3L4HZ5aUay1Krdah1g1g2i4NJdy59zK6RASXQ2XquyAJTVavtqYPeHzysyZgv2h3xA43Xu7i2kEbmc3jGjp5k5KoZEG84peABdh2smnUKcxVzhFFmTc4PE22L2eHBMNtGkcXzES1pkWgn1PZuvRZf1jwT6yBStjYhMJFd7FjvGSV1P8FGd8ht3D37VWfikkXpBni5jANdG8PcW6uSunNq4vGYnYZwXqLYEaFGJ36fTSeA52sWCNE9nB6NkHcH3cgcXiRt6PvAUmV5Hn73PegFipDAo7TUU7WqLKpX7gMD3p4DFiWLKKzNQ1q4uJAt5";
            byte[] decodedBytes = Base58CheckEncoding.Decode(QRCode);
            string message = Encoding.Default.GetString(decodedBytes);

            P2PPairingResponse pairingResponse = JsonConvert.DeserializeObject<P2PPairingResponse>(message);

            var beaconPeer = new BeaconPeer(
                pairingResponse!.Id,
                pairingResponse.Name,
                pairingResponse.PublicKey,
                pairingResponse.RelayServer,
                pairingResponse.Version,
                null,
                null);

            // var seed = Guid.NewGuid().ToString();
            const string seed = "44cf34fa-b4d4-ec89-cb5c-22e14f6156c6";
            // var public = BeaconCryptographyService.ToHexString(Encoding.Default.GetBytes(seed));

            // BeaconCryptographyService.ToHexString(seed);
            KeyPair keyPair = CryptographyService.GenerateEd25519KeyPair(seed);

            MatrixClient matrixClient = serviceProvider.GetRequiredService<MatrixClient>();
            IChannelOpeningMessageBuilder channelOpeningMessageBuilder =
                serviceProvider.GetRequiredService<IChannelOpeningMessageBuilder>();

            var clientOptions = new P2PClientOptions("Test App Name", "beacon-node-0.papers.tech:8448");
            var client = new P2PCommunicationCommunicationClient(matrixClient, channelOpeningMessageBuilder);

            await client.StartAsync(keyPair);
            await client.SendChannelOpeningMessageAsync(beaconPeer);

            Console.ReadLine();
        }

        private static async Task<(MatrixClient, TextMessageListener)> SetupClientWithTextListener(
            IServiceProvider serviceProvider)
        {
            MatrixClient matrixClient = serviceProvider.GetRequiredService<MatrixClient>();
            var seed = Guid.NewGuid().ToString();
            KeyPair keyPair = Matrix.Sdk.Core.Infrastructure.Services.CryptographyService.GenerateEd25519KeyPair(seed);

            await matrixClient.StartSync(keyPair); //Todo: generate once and then store seed?

            var textMessageListener = new TextMessageListener(matrixClient.UserId, (listenerId, textMessageEvent) =>
            {
                (string roomId, string senderUserId, string message) = textMessageEvent;
                if (listenerId != senderUserId)
                    Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
            });

            textMessageListener.ListenTo(matrixClient.MatrixEventNotifier);

            return (matrixClient, textMessageListener);
        }
    }
}

// var publicKey = BeaconCryptographyService.ToHexString(keyPair.PublicKey);