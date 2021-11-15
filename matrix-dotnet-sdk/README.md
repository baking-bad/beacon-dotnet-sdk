# **Matrix .NET SDK**

This open-source library allows you to build .NET apps compatible with Matrix Protocol
(http://www.matrix.org).
It has support for a limited subset of the APIs presently. 

This SDK was built for interaction with the Beacon Node (https://github.com/airgap-it/beacon-node). It supports login through the crypto auth provider (https://github.com/airgap-it/beacon-node/blob/master/docker/crypto_auth_provider.py).  
# Use the SDK in your app

## Simple usage

## Create
```cs
var factory = new MatrixClientFactory();
IMatrixClient client = firstFactory.Create();
```
## Attach Event Listener
```cs
client.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
{
    foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
    {
        if (roomEvent is not TextMessageEvent textMessageEvent)
            continue;

        (string roomId, string senderUserId, string message) = textMessageEvent;
        if (client.UserId != senderUserId)
            Console.WriteLine($"RoomId: {roomId} received message from {senderUserId}: {message}.");
    }
};
```
## Login
Logic for key pair generation.
```cs
var seed = Guid.NewGuid().ToString();
KeyPair keyPair = CryptographyService.GenerateEd25519KeyPair(seed);

byte[] loginDigest = CryptographyService.GenerateLoginDigest();
string hexSignature = CryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
string publicKeyHex = CryptographyService.ToHexString(keyPair.PublicKey);
string hexId = CryptographyService.GenerateHexId(keyPair.PublicKey);

var password = $"ed:{hexSignature}:{publicKeyHex}";
string deviceId = publicKeyHex;

var baseAddress = new Uri("https://beacon-node-0.papers.tech:8448/");

LoginRequest loginRequest = new LoginRequest(baseAddress, hexId, password, deviceId);
```

Login into Matrix
```cs
await firstClient.LoginAsync(loginRequest);
```

## Stop client

```cs
client.Stop();
```
