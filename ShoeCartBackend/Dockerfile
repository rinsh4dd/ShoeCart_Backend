# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the project and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Use dynamic port for hosting platforms (Render, Heroku, etc.)
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 5000

# Run the app
ENTRYPOINT ["dotnet", "ShoeCartBackend.dll"]
