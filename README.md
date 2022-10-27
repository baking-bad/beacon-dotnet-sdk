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

For a complete example, refer
to [`Dapp sample`](https://github.com/baking-bad/beacon-dotnet-sdk/blob/main/Beacon.Sdk.Sample.Dapp/Sample.cs)
or to [`Wallet sample`](https://github.com/baking-bad/beacon-dotnet-sdk/blob/main/Beacon.Sdk.Sample.Wallet/Sample.cs).

## **Wallet**

Here is step by step **guide** how to create and use `WalletBeaconClient`:

#### 1. Create

Use `BeaconClientFactory` to create an instance of `WalletBeaconClient`

```cs
const string path = "wallet-beacon-sample.db";

var factory = new WalletBeaconClientFactory();

var options = new BeaconOptions
{
    AppName = "Wallet sample",
    AppUrl = "https://awesome-wallet.io",
    IconUrl = "https://services.tzkt.io/v1/avatars/KT1TxqZ8QtKvLu3V3JH7Gx58n7Co8pgtpQU5",
    KnownRelayServers = Constants.KnownRelayServers,

    // for some operating systems compability reasons we should use Mode=Exclusive for LiteDB.
    DatabaseConnectionString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? $"Filename={path}; Connection=Shared;"
        : $"Filename={path}; Mode=Exclusive;"
};

// creating test logger, you can provide your own app-context logger here.
Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

ILoggerProvider loggerProvider = new SerilogLoggerProvider(Logger);

IWalletBeaconClient beaconWalletClient = BeaconClientFactory.Create<IWalletBeaconClient>(options, loggerProvider);
```

#### 2. Start listening for incoming events

To listen for the incoming Beacon messages you need to subscribe to `OnBeaconMessageReceived;`

```cs
beaconWalletClient.OnBeaconMessageReceived += async (object? sender, BeaconMessageEventArgs args) =>
{
    BaseBeaconMessage message = args.Request;
    if (message == null) return;

    case BeaconMessageType.permission_request:
    {
        if (message is not PermissionRequest permissionRequest)
            return;

        // here we give all permissions that dApp request, you can modify permissionRequest.Scopes
        var response = new PermissionResponse(
            id: permissionRequest.Id,
            senderId: beaconWalletClient.SenderId,
            appMetadata: beaconWalletClient.Metadata,
            network: permissionRequest.Network,
            scopes: permissionRequest.Scopes,
            publicKey: "your tezos wallet address public key",
            version: permissionRequest.Version);
            
        await beaconWalletClient.SendResponseAsync(receiverId: permissionRequest.SenderId, response);
        break;
    }

    case BeaconMessageType.sign_payload_request:
    {
        if (message is not SignPayloadRequest signRequest)
            return;

        // do some stuff here and send response. 
        break;
    }

    case BeaconMessageType.operation_request:
    {
        if (message is not OperationRequest operationRequest)
            return;

        // do some stuff here and send response. 
        break;
    }

    default:
    {
        var error = new BeaconAbortedError(
            id: KeyPairService.CreateGuid(),
            senderId: beaconWalletClient.SenderId);

        await beaconWalletClient.SendResponseAsync(receiverId: message.SenderId, error);
        break;
    }
};
```

You can also subscribe to

```cs
beaconWalletClient.OnConnectedClientsListChanged += (object sender, ConnectedClientsListChangedEventArgs e) => {}
```

This event is triggered at the moments of issuing permissions or disconnecting peers, it's good to change your
connected apps list here.

I recommend that you don't use anonymous functions to subscribe to events if you have to unsubscribe from the event at
some later point in your code.

#### 3. Init

```cs
await beaconWalletClient.InitAsync()
```

#### 4. Connect

```cs
beaconWalletClient.Connect();
```

#### 5. Add Peer

```cs
string pairingQrCode = "paste-qrcode-here";

var pairingRequest = beaconWalletClient.GetPairingRequest(pairingQrCode);
await beaconWalletClient.AddPeerAsync(pairingRequest);
```

#### 6. Disconnect

```cs
beaconWalletClient.Disconnect();
```

## Demo app

Follow these steps to reproduce the typical wallet workflow:

1. Clone this repo and restore nuget packages
2. Open Beacon [playground](https://docs.walletbeacon.io/getting-started/first-dapp#setup), scroll to Setup and press "
   Run Code"
3. Choose "Pair wallet on another device" and click on the QR code to copy
4. Start `Beacon.Sdk.Sample.Wallet` sample project
5. Paste copied QR code to console
6. In the browser you should see "Got permissions" message, sample project response with all requested permissions to
   dApp
7. Scroll down to "Operation Request" item and do the "Run Code" thing again
8. You should see the successful operation message

## **Dapp**

Here is step by step **guide** how to create and use `DappBeaconClient`:

#### 1. Create

Use `BeaconClientFactory` to create an instance of `DappBeaconClient`

```cs
IDappBeaconClient beaconDappClient = BeaconClientFactory.Create<IDappBeaconClient>(options, loggerProvider);
```

Options are same as during `WalletBeaconClient` creation

#### 2. Start listening for incoming events

To listen for the incoming responses from wallet you need to subscribe to `OnBeaconMessageReceived`

```cs
beaconDappClient.OnBeaconMessageReceived += async (object? sender, BeaconMessageEventArgs args) =>
{
     if (args.PairingDone)
     {
         var network = new Network
         {
             Type = NetworkType.mainnet,
             Name = "mainnet",
             RpcUrl = "https://rpc.tzkt.io/mainnet"
         };

         var permissionScopes = new List<PermissionScope>
         {
             PermissionScope.operation_request,
             PermissionScope.sign
         };

         // after pairing complete we request permissions from wallet
         await BeaconDappClient.RequestPermissions(permissionScopes, network);
         return;
     }
     
     var message = args.Request;
     if (message == null) return;
     
     // almost like as in WalletClient, but we receiving Reponses here and sending Requests
     switch (message.Type)
     {
         case BeaconMessageType.permission_response:
         {
             if (message is not PermissionResponse)
                 return;
                 
             // do some stuff here and send response.
             break;
         }
         
         case BeaconMessageType.operation_response:
         // ...
         
         case BeaconMessageType.sign_payload_response:
         // ...
     }
};
```

`OnConnectedClientsListChanged` also available as in `DappBeaconClient`. They both inherited from `BaseBeaconClient`
class

#### 3. Init

```cs
await beaconDappClient.InitAsync()
```

#### 4. Connect

```cs
beaconDappClient.Connect();
```

#### 5. Get pairing request data if we need pairing

```cs
var pairingRequestQrData = beaconDappClient.GetPairingRequestInfo();
```

Copy `pairingRequestQrData` and paste it to Beacon wallet

#### 6. Try get active account

```cs
var activeAccountPermissions = beaconDappClient.GetActiveAccount();
```

#### 7. If active account is not null we can

```cs
beaconDappClient.RequestPermissions(permissionScopes, network);
beaconDappClient.RequestSign(PayloadToSign, SignPayloadType.raw);
beaconDappClient.RequestOperation(operationDetails);
```

#### 8. Disconnect

```cs
beaconDappClient.Disconnect();
```

## Demo app

Follow these steps to reproduce the typical wallet workflow:

1. Clone this repo and restore nuget packages
2. Start `Beacon.Sdk.Sample.Dapp` project
3. Copy pairing data string from console and paste it to Beacon wallet
4. You should see that Beacon wallet received permissions from `Beacon.Sdk.Sample.Dapp`
5. You can print `sign` command in console to send `sign request` to the wallet
6. You can print `operation` command in console to send `operation request` to the wallet

Take a look
at [`Dapp sample`](https://github.com/baking-bad/beacon-dotnet-sdk/blob/main/Beacon.Sdk.Sample.Dapp/Sample.cs)
or at [`Wallet sample`](https://github.com/baking-bad/beacon-dotnet-sdk/blob/main/Beacon.Sdk.Sample.Wallet/Sample.cs)
for complete examples