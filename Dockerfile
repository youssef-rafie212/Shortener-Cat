# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build 

WORKDIR /src 

COPY ./ShortenerCat.sln ./
COPY ["./Shortener Cat/Shortener Cat.csproj", "./Shortener Cat/"]
COPY ./Core/Core.csproj ./Core/
COPY ./Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY ./Tests/Tests.csproj ./Tests/

RUN dotnet restore

COPY . .

WORKDIR "/src/Shortener Cat"

RUN dotnet build -c debug -o /src/out


# run stage 
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run 

WORKDIR /app

COPY --from=build ["/src/out", "./"] 

EXPOSE 6969

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:6969

ENTRYPOINT ["dotnet", "Shortener Cat.dll"]



