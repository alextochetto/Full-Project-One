FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . "IdentityProvider/"
WORKDIR "/src/IdentityProvider"
RUN dotnet restore "IdentityProvider.csproj"
RUN dotnet build "IdentityProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityProvider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityProvider.dll"]