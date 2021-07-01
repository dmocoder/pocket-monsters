# Pocket Monsters
Pocket Monsters is an Api that retrieves Pokemon Details and provides Translations of their descriptions.

>### Limitations
>Pocket Monsters only supports English language descriptions of Pokemon

## How to Run
Pocket Monsters requires .NET 5.0 SDK to build & run. The SDK for your Operating System can be downloaded [here](https://dotnet.microsoft.com/download). 

With .NET 5 installed, the Api can be built and run using the following command from the solution directory:

`dotnet run --project ./src/PocketMonsters/PocketMonsters.csproj`

The following url can then be used to check that the Api is Healthy: `http://localhost:5000/health`

### Using Docker
The Api can also be hosted using Docker. Navigate to the project folder (./src/PocketMonsters) and run the following:

Build the image:

`docker build -t pocketmon .`

Run the image:

`docker run -it --rm -p 5000:80 --name pocketmon_run pocketmon`

As above, the Api health check can again be called using: `http://localhost:5000/health`

### Https Redirection
The Api redirects Http requests to Https and uses a HTTPS Development Certificate. 
When calling the Api there may be a prompt or exception regarding untrusted certificates.
For Windows and Mac OS this can be alleviated by trusting the certificate using the 
[instruction](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos) outlined by Microsoft.

After installing the dotnet SDK the certificate can be trusted using the following command: 

`dotnet dev-certs https --trust`

## Gotta GET 'Em All

Once the Api is running, the description of the Pokemon can be retrieved by calling the Pokemon endpoint using a GET request or by using your browser and supplying the Pokemon name:
`http://localhost:5000/pokemon/{pokemonName}`

For example:

```
http://localhost:5000/pokemon/snorlax
```

Will return:

```
{
    "name": "snorlax",
    "description": "Very lazy. Just eats and sleeps. As its rotund bulk builds, it becomes steadily more slothful.",
    "habitat": "mountain",
    "isLegendary": false,
    "apiVersion": "v1"
}
```

### Translated Description
To retrieve a Pokemon with a translated description, call the Translated endpoint supplying the Pokemon name using your browser or a GET request: `http://localhost:5000/pokemon/translated/{pokemonName}`

For example:

```http://localhost:5000/pokemon/translated/snorlax```

Will return:
```
{
    "name": "snorlax",
    "description": "Very distemperate. Just engluts and sleeps. As its rotund bulk builds,  't becomes steadily moo slothful.",
    "habitat": "mountain",
    "isLegendary": false
}
```


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

# What would I do differently for a Production Api?
#### Idempotency
  To respect REST verbs, Api should be idempotent about each GET endpoint. 
  To facilitate this, the Api should ideally cache the request and response when successful. 
  
Additionally, the PokeDex service currently uses the first English flavor text returned by the PokeApi. My testing suggested this was always ordered correctly but without ordering in this application there is a chance that a pokemon description request may not be idempotent; i.e. the description is different on repeat requests.

To alleviate this, the service should order flavor text in some order of preference for version so that the same description would be returned for a repeat request. This was not added to this solution because (as mentioned above) the ordering from the PokeApi appeared to be the same with each request, and also given the many pokemon versions available, testing this functionality would require an exhaustive suite of tests that seemed beyond the scope of this project.

#### Validate Pokemon Names when Species endpoint returns 404
In its current guise, the PokeDex service returns 404 if the call to the PokeApi species endpoint returned 404.
This assumes that the Pokemon name is the same as the Pokemon Species name, which is not strictly true. 
A better solution would be to, upon failure, call into the Pokemon endpoint and retrieve the Pokemon Species name from the response body and _then_ call the species endpoint; only returning 404 if this subsequent check fails.

This was not included in this project as it added additional complexity that for the purpose of this submission seemed out of scope.


#### Extend Error Responses
The current Error responses are limited mostly to the error codes. Ideally more information should be provided to an end user.

The PokeDex service also does not currently consider specific error responses outside of 404 and these should be catered for.

#### Add Authentication & Rate Limiting
Depending on the Api requirements, some layer of Authentication should be included to restrict usage. Given this Api calls into other external Apis, there is a risk that high volume of requests may hit rate limits in these external apis (this is already the case with the translation service).

#### Appropriate Health Checks
The Api Health checks currently only test that the Api is running. Realistically, a more useful check would be to test the external Apis as the service is highly dependent on their availability.

#### TLS Signing
The Api uses HTTPS developer certificates and for a production application a better solution should be setup.

#### Add a Simulator
Currently tests call into the actual PokeApi and Translation services. It might be better long term to introduce a simulator that mocks their responses.

#### Version Headers
In the response dto I have added a version field. In production this would ideally be added via middleware to the response header instead of a body field.

#### Swagger Documentation
For a reason I wasn't able to figure out in time, the XML docs are not picked up by the Swagger documentation, and as such the swagger documentation is underwhelming.
For a production application this would be resolved so that each response type had good documentation.