# Setup

The app requires setting up the identity provider configuration and the environment variables before it can be run.

## Add External Identity Provider Configuration

Add a `identity-providers.json` to `./GIAP.Server/Configuration/` with the following properties.

### Configuration Properties

| Name                       | Required | Explanation                                                                                                                                                                                                                                                                            |
|----------------------------|----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `name`                     | Yes      | The name used to display within the UI. It doesn't have to be unique like the Slug.                                                                                                                                                                                                    |
| `slug`                     | Yes      | Slug must be unique across all identity providers. Used for web url paths and as the OpenID Connect scheme name.                                                                                                                                                                       |
| `openIdWellKnownUrl`       | Yes      | The full URL to the .well-known/openid-configuration endpoint of the identity provider.                                                                                                                                                                                                |
| `clientId`                 | Yes      | The client ID registered at the identity provider.                                                                                                                                                                                                                                     |
| `clientSecret`             | Yes      | The client secret registered at the identity provider.                                                                                                                                                                                                                                 |
| `callbackPath`             | Yes      | The callback path registered at the identity provider. Used to redirect the user to after successful authentication. Also known as the redirect URI. Must be unique for each identity provider due to Microsoft.AspNetCore.Authentication.OpenIdConnect.                               |
| `schemePath`               | Yes      | The scheme path used to build the credential scheme URL.                                                                                                                                                                                                                               |
| `issuanceValidityInMonths` | Yes      | Validity in months of the issued credential in the Yivi app.                                                                                                                                                                                                                           |
| `attributeMapping`         | Yes      | Mapping of attribute ID's received from the identity provider to the attributes used in the credential. The JWT the identity provider returns is expected to return key-value pairs. To support getting nested values, in the future this could also include JSON pointers or similar. |
| `apiUrls`                  | No       | Optional list of API URLs to get additional attributes required for issuing a credential. The access token from the identity provider will be used to call these API URLs. The API URLs are expected to return JSON with key-value pairs.                                              |

### Example Identity Provider Configuration

Example (from `./GIAP.Server/Configuration/Examples`):

```json
[
  {
    "name": "Example Identity Provider 1",
    "slug": "example-1",
    "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
    "clientId": "123-456-789",
    "clientSecret": "ABC-123-XYZ",
    "callbackPath": "/api/callback-signin-example-1",
    "apiUrls": [
      "https://api.example.local/me?fields=id,given_name,family_name,companyName"
    ],
    "schemePath": "pbdf/Issues/example1/description.xml",
    "issuanceValidityInMonths": 6,
    "attributeMapping": {
      "id": "id",
      "given_name": "givenName",
      "family_name": "surname",
      "companyName": "companyName"
    }
  },
  {
    "name": "Example Identity Provider 2",
    "slug": "example-2",
    "openIdWellKnownUrl": "https://example.local/.well-known/openid-configuration",
    "clientId": "987-654-321",
    "clientSecret": "ZYX-321-CBA",
    "callbackPath": "/api/callback-signin-example-2",
    "schemePath": "pbdf/Issues/example2/description.xml",
    "issuanceValidityInMonths": 6,
    "attributeMapping": {
      "http://example.local/claims/id": "id",
      "given_name": "givenName",
      "family_name": "surname"
    }
  }
]
```

## Add Environment Variables Configuration

Add a `.env` to `./GIAP.Server/` with the following properties.

### Configuration Properties

| Name                    | Required | Explanation                                                                                                                  |
|-------------------------|----------|------------------------------------------------------------------------------------------------------------------------------|
| `IRMA_SERVER_BASE_URL`  | Yes      | The IRMA base url such as "https://is.staging.yivi.app".                                                                     |
| `IRMA_SERVER_API_TOKEN` | Yes      | IRMA Server API token to authenticate with the IRMA Server.                                                                  |
| `SCHEME_BASE_URL`       | Yes      | Scheme base URL used to build the full URL for the scheme. A scheme base URL looks like: "https://schemes.staging.yivi.app". |
| `SCHEME_NAME`           | Yes      | Scheme name used to build the full URL for the scheme.                                                                       |

### Example .env file

Example (from `./GIAP.Server/.env.example`):

```dotenv
IRMA_SERVER_BASE_URL=https://is.staging.example.com
IRMA_SERVER_API_TOKEN=example-server-api-token
SCHEME_BASE_URL=https://schemes.staging.yivi.app
SCHEME_NAME=pbdf-staging
```

# Development

## Prerequisites

You'll need to be able to run the following:

* **Docker**: to build the Docker image and run the Docker container.
* **.NET 8**: for backend development.
* **npm**: for frontend development.

## Frontend & Backend

Run the backend server (this should install any missing dependencies):

```
dotnet watch
```

`Microsoft.AspNetCore.SpaProxy` will automatically start the frontend development server when you run the backend
server with hot reload enabled for both the frontend and backend.

# Tests

Navigate to the test directory:

```
cd GIAP.Tests
```

Run the tests:

```
dotnet test
```

## Build

Build the image:

```
docker build -t giap-image -f Dockerfile .
```

Run the container and remove it when it stops:

```
docker run --rm -p 62858:8080 --name giap-container giap-image
```
