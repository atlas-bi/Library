
FROM microsoft/dotnet:2.2-sdk-alpine3.8 AS build
WORKDIR /app
COPY Data_Governance_WebApp.sln .
COPY ["./Data Governance WebApp/Data Governance WebApp.csproj", "./Data Governance WebApp/"]
RUN ls "./Data Governance WebApp/"
RUN dotnet restore

COPY ["./Data Governance WebApp/.", "./Data Governance WebApp/"]
WORKDIR "/app/Data Governance WebApp/"
RUN dotnet publish -c Release -o out

# to copy linux build back to host
# 
# 1 build
# docker build --tag atlas .
# 
# 2. run container
# docker run -i -t -u 0 atlas:latest
#
# 3. get container id
# docker container ls
#
# 4. copy file
# docker cp <containerId>:/path/on/container /path/on/host