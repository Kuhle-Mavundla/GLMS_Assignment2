FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore for better caching
COPY ["GLMS_Assignment2/GLMS_Assignment2.csproj", "GLMS_Assignment2/"]
RUN dotnet restore "GLMS_Assignment2/GLMS_Assignment2.csproj"

# copy everything and publish
COPY . .
WORKDIR /src/GLMS_Assignment2
RUN dotnet publish "GLMS_Assignment2.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ENV ASPNETCORE_URLS=http://+:80
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS_Assignment2.dll"]