# Pocket Monsters
Pocket Monsters is an Api that retrieves Pokemon Details and provides Translations of their descriptions.

>### Limitations
>Pocket Monsters only supports English language descriptions of Pokemon

## How to Run
Pocket Monsters requires .NET 5.0 SDK to build & run. The SDK for your Operating System can be downloaded [here](https://dotnet.microsoft.com/download). 

With .NET 5 installed, the Api can be built and run using the following command from the solution directory:
`dotnet run --project ./src/PocketMonsters/PocketMonsters.csproj`

The following url be used to check that the Api is up and running: `http://localhost:5000/health`

### Using Docker
The Api can also be hosted using Docker. Navigate to the project folder (./src/PocketMonsters) and run the following:

Build the image:

`docker build -t pocketmon .`

Run the image:

`docker run -it --rm -p 5000:80 --name pocketmon_run pocketmon`

## Documentation

The Api provides Open API Specification. Once the app is running this can be accessed at the following endpoint: 

`https://localhost:5001/swagger/index.html` 

N.B. When running the Application from an IDE such as Visual Studio or Rider, this documentation page will load by default.

## Unit & Integration Tests

The solution includes a number of unit tests and integration tests. The latter of these call into external Apis but the tests can be excluded using the following commands:

#### From the Solution Root:
To run only Unit tests:
`dotnet test --filter Category=Unit`

To run both Unit & Integration tests and exclude tests against services that are rate limited:
`dotnet test --filter Category!=RateLimited`

