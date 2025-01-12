#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Panopticon/Panopticon.csproj", "Panopticon/"]
RUN dotnet restore "Panopticon/Panopticon.csproj"
COPY ["Panopticon.Data/Panopticon.Data.csproj","Panopticon.Data/"]
RUN dotnet restore "Panopticon.Data/Panopticon.Data.csproj"
COPY ["Panopticon.Shared/Panopticon.Shared.csproj","Panopticon.Shared/"]
RUN dotnet restore "Panopticon.Shared/Panopticon.Shared.csproj"
COPY . .
WORKDIR "/src/Panopticon"
RUN dotnet build "Panopticon.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Panopticon.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Panopticon.dll"]
 