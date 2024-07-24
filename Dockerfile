FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY . .

RUN dotnet build --configuration Release
RUN dotnet publish --configuration Release --output ./publish

WORKDIR /src/publish

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

ENTRYPOINT ["dotnet", "Manmudra.GroupsAPI.dll"]
