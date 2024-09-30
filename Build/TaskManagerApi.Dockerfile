# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

COPY Source/Api/TaskManagerApi/aspnetapp.pfx /etc/ssl/certs/aspnetapp.pfx

# Expose port 8080 instead of 80 and 443 if you want the app to bind to it
EXPOSE 8080
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Source/Api/TaskManagerApi/TaskManagerApi.csproj", "Source/Api/TaskManagerApi/"]
RUN dotnet restore "Source/Api/TaskManagerApi/TaskManagerApi.csproj"
COPY . .
WORKDIR "/src/Source/Api/TaskManagerApi"
RUN dotnet build "TaskManagerApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TaskManagerApi.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerApi.dll"]
#ENTRYPOINT ["tail", "-f", "/dev/null"]
