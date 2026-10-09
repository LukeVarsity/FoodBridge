# Same multi-stage pattern as the API. Build with the SDK image,
# run on the smaller ASP.NET runtime image.
#
# Containerize a .NET app
# Microsoft Learn
# Accessed : 9 October 2026
# Available at : https://learn.microsoft.com/en-us/dotnet/core/docker/build-container

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY FoodBridge.Web.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENV TZ=Africa/Johannesburg

EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "FoodBridge.Web.dll"]
