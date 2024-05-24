# Use Microsoft's official SDK image to create a build artifact.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies.
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build the application.
COPY . ./
RUN dotnet publish -c Release -o out

# Generate the runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyGymProgressApi.dll"]
