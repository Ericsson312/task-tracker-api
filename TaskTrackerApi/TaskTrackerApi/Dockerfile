# specify the target SDK (base image)
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

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

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "TaskTrackerApi.dll"]
