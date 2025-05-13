# Setup

Add a `identity-providers.json` to `./GIAP.Server/Configuration/`.
Example (from `./GIAP.Server/Configuration/Examples`):

```json
[
  {
    "name": "Identity Provider Name",
    "slug": "idp-web-url-slug"
  }
]

```

# Development

## Prerequisites

You'll need to be able to run the following:

* **Docker**: to build the Docker image and run the Docker container.
* **.NET 8**: for backend development.
* **npm**: for frontend development.

## Frontend

Navigate to the frontend directory:

```
cd giap.client
```

Install dependencies:

```
npm install
```

Run the frontend dev server:

```
npm run dev
```

## Backend

Navigate to the backend directory:

```
cd GIAP.Server
```

Run the backend server (dotnet watch will automatically get any NuGet dependencies):

```
dotnet watch
```

# Tests

Navigate to the test directory:

```
cd GIAP.Tests
```

Run the tests:

```
dotnet run
```

## Build

Navigate to the backend server directory:

```
cd GIAP.Server
```

Build the image:

```
docker build -t giap-image -f Dockerfile
```

Run the container:

```
docker run -d -p 62858:8080 --name giap-container giap-image
```
