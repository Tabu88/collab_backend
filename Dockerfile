FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
WORKDIR /App

COPY . ./

RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_ENVIRONMENT=Staging
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "AKIMotorsGateway.dll"]
EXPOSE 80
EXPOSE 3000/tcp