FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Server.csproj ./
RUN dotnet restore "Server.csproj"

COPY . ./

RUN dotnet publish "Server.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

COPY ./certs ./certs

EXPOSE 5001

ENV ASPNETCORE_URLS=https://0.0.0.0:5001
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Server.dll"]