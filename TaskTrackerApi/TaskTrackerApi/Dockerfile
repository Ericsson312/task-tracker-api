# specify the target SDK (base image)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

# copy .csproj file to build directory 
COPY TaskTrackerApi.csproj /build/

# restore all dependancies for .csproj file in build directory
RUN dotnet restore ./build/TaskTrackerApi.csproj

# copy all files to build directory
COPY . ./build/
# set build folder as working directory 
WORKDIR /build/
# publish API
RUN dotnet publish ./TaskTrackerApi.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "TaskTrackerApi.dll"]
