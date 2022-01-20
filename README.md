# **Beacon .NET SDK**
![beacon-logo](beacon-logo.svg)

[Beacon](https://walletbeacon.io) is the implementation of the wallet interaction
standard [tzip-10](https://gitlab.com/tzip/tzip/blob/master/proposals/tzip-10/tzip-10.md) which describes the
connnection of a dApp with a wallet.

## Supported Platforms

* .NET Standard 2.1

## Installation

Beacon .NET SDK is [available on NuGet](https://www.nuget.org/packages/Beacon.Sdk/):

```
dotnet add package Beacon.Sdk
```
## Use the SDK in your app
For a complete example, refer to [`Sample.cs`](https://github.com/baking-bad/beacon-dotnet-sdk/blob/main/Beacon.Sdk.Sample.Console/Sample.cs).
You can also clone this repository and run `Beacon.Sdk.Sample.Console`.


### **Wallet**
Here is step by step **guide**:

#### 1. Create
Use `WalletBeaconClientFactory` to create an instance of `WalletBeaconClient`
```cs
const string path = "your-database-name.db";

var factory = new WalletBeaconClientFactory();

var options = new BeaconOptions
{
    AppName = "your-app-name",
    AppUrl = "optional-app-url", //string?
    IconUrl = "optional-icon-url", // string?
    DatabaseConnectionString = $"Filename={path}"
};

IWalletBeaconClient walletClient = factory.Create(options);
```
#### 2. Start listening for incoming events
To listen for the incoming Beacon messages you need to subscribe to `OnBeaconMessageReceived;`
```cs
walletClient.OnBeaconMessageReceived += async (_, dAppClient) =>
{
    BaseBeaconMessage message = dAppClient.Request;

    if (message.Type == BeaconMessageType.permission_request)
    {
        var request = message as PermissionRequest;
        
        var network = new Network 
        {
            Type = NetworkType.hangzhounet,
            Name = "Hangzhounet",
            RpcUrl = "https://hangzhounet.tezblock.io"
        };

        var response = new PermissionResponse(
            id: request!.Id,
            senderId: walletClient.SenderId,
            network: network,
            scopes: request.Scopes,
            publicKey: walletKey.PubKey.ToString(),
            appMetadata: walletClient.Metadata);

        // var response = new BeaconAbortedError(message.Id, walletClient.SenderId);

        await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
    }
    else if (message.Type == BeaconMessageType.operation_request)
    {
        var request = message as OperationRequest;
        
        string transactionHash = await MakeTransactionAsync(walletKey);

        var response = new OperationResponse(
            id: request!.Id,
            senderId: walletClient.SenderId,
            transactionHash: transactionHash);

        await walletClient.SendResponseAsync(receiverId: dAppClient.SenderId, response);
    }
};
```
I recommend that you don't use anonymous functions to subscribe to events if you have to unsubscribe from the event at some later point in your code.

#### 3. Init

```cs
await walletClient.InitAsync()
```

#### 4. Connect
```cs
walletClient.Connect();
```

## 5. Add Peer
```cs
string QrCode = "paste-qrcode-here";

byte[] decodedBytes = Base58CheckEncoding.Decode(QrCode);
string message = Encoding.Default.GetString(decodedBytes);

P2PPairingRequest pairingRequest = JsonConvert.DeserializeObject<P2PPairingRequest>(message);

await walletClient.AddPeerAsync(pairingRequest!);
```

#### 6. Disconnect
```cs
walletClient.Disconnect();
```

## Demo app

Follow these steps to reproduce the typical wallet workflow:
1. Clone this repo and make sure you have `dotnet` command available in your shell
2. Open Beacon [playground](https://docs.walletbeacon.io/getting-started/first-dapp#setup), scroll to Setup and press "Run Code"
3. Choose "Pair wallet on another device" and click on the QR code to copy
4. In your shell run `make sample` and follow the instructions
5. In the browser you should see "Got permissions" message
6. Scroll down to "Operation Request" item and do the "Run Code" thing again
7. You should see the sucessful operation message