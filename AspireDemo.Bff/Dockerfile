FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AspireDemo.Bff/AspireDemo.Bff.csproj", "AspireDemo.Bff/"]
COPY ["AspireDemo.ServiceDefaults/AspireDemo.ServiceDefaults.csproj", "AspireDemo.ServiceDefaults/"]
RUN dotnet restore "./AspireDemo.Bff/AspireDemo.Bff.csproj"
COPY . .
WORKDIR "/src/AspireDemo.Bff"
RUN dotnet build "./AspireDemo.Bff.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AspireDemo.Bff.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:20.12 AS frontend
WORKDIR /app
COPY ["AspireDemo.Web/package.json", "AspireDemo.Web/"]
COPY ["AspireDemo.Web/package-lock.json", "AspireDemo.Web/"]
WORKDIR "/app/AspireDemo.Web"
RUN npm install
WORKDIR /app
COPY . .
WORKDIR "/app/AspireDemo.Web"
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=frontend /app/AspireDemo.Web/dist wwwroot
ENTRYPOINT ["dotnet", "AspireDemo.Bff.dll"]