FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env
WORKDIR /app

COPY AzureBuildsBrowser/*.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000  
EXPOSE 5000
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AzureBuildsBrowser.dll"]
