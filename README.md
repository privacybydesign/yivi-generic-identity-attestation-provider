# Development

## Frontend
Navigate to the frontend directory:
```
cd giap.client
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

Run the backend server:
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
docker build -t giap-image -f Dockerfile ..
```

Run the container:
```
docker run -d -p 62858:8080 --name giap-container giap-image
```
