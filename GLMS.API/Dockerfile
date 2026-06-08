FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore for better caching
COPY ["GLMS.API/GLMS.API.csproj", "GLMS.API/"]
RUN dotnet restore "GLMS.API/GLMS.API.csproj"

# copy everything and publish
COPY . .
WORKDIR /src/GLMS.API
RUN dotnet publish "GLMS.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS.API.dll"]