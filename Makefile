install:
	dotnet restore

clean:
	dotnet clean

wallet-sample:
	dotnet run -p Beacon.Sdk.Sample.Wallet -v normal

dapp-sample:
	dotnet run -p Beacon.Sdk.Sample.Dapp -v normal