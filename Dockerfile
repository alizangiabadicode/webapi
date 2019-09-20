FROM microsoft/dotnet:2.2-sdk AS build-env

WORKDIR /app



# Copy csproj and restore as distinct layers

COPY *.csproj ./

RUN dotnet restore


# Copy everything else and build

COPY . ./

RUN dotnet publish -c Release -o out



# Build runtime image

FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app/bin/Release/netcoreapp2.2/

#COPY  /app/bin/Release/netcoreapp2.2/ /app

COPY /datedb /app/bin/Release/netcoreapp2.2

CMD dotnet datingapp.api.dll