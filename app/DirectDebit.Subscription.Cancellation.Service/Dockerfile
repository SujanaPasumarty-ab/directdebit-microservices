# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

# App Service expects the container to listen on port 80
EXPOSE 8080
# Force ASP.NET to bind to port 80
ENV ASPNETCORE_URLS=http://+:8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DirectDebit.Subscription.Cancellation.Service/DirectDebit.Subscription.Cancellation.Service.csproj", "DirectDebit.Subscription.Cancellation.Service/"]
COPY ["DirectDebit.Subscription.Cancellation.Service.Library/DirectDebit.Subscription.Cancellation.Service.Library.csproj", "DirectDebit.Subscription.Cancellation.Service.Library/"]
RUN dotnet restore "./DirectDebit.Subscription.Cancellation.Service/DirectDebit.Subscription.Cancellation.Service.csproj"
COPY . .
WORKDIR "/src/DirectDebit.Subscription.Cancellation.Service"
RUN dotnet build "./DirectDebit.Subscription.Cancellation.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DirectDebit.Subscription.Cancellation.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DirectDebit.Subscription.Cancellation.Service.dll"]
