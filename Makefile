install:
	dotnet restore

clean:
	dotnet clean

sample:
	dotnet run -p Beacon.Sdk.Sample.Console -v normal