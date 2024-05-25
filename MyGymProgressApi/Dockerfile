# Use Microsoft's official SDK image to create a build artifact.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies.
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build the application.
COPY . ./
RUN dotnet publish -c Release -o out
RUN ls -la /app/out

# Generate the runtime image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MyGymProgressApi.dll"]
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

## Define the build stage
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#COPY ["MyGymProgressApi.csproj", "./"]
#RUN dotnet restore "MyGymProgressApi.csproj"
#COPY . .
#RUN dotnet build "MyGymProgressApi.csproj" -c Release -o /app/build
#
## Define the publish stage
#FROM build AS publish
#RUN dotnet publish "MyGymProgressApi.csproj" -c Release -o /app/publish
#
## Define the final stage based on the .NET runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#EXPOSE 80
#ENTRYPOINT ["dotnet", "MyGymProgressApi.dll"]
#