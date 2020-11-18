# to build
# docker build  --tag atlas_demo . 

# to run locally
# docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 atlas_demo:latest
# or 
# docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest

# to run online 
# https://labs.play-with-docker.com
# click start, paste in command below. After startup (about 1 min), click "open port 1234" in menu bar.
# docker run -i -t -p 1234:1234 -e PORT=1234  -u 0 christopherpickering/rmc-atlas-demo:latest

# to access db
# docker ps (get running container id)
# docker exec -it <running_cont_id> bash  
# /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'p@ssw0rd' -d DATA_GOVERNANCE
# to run sql, enter command, hit enter, then type "go" and hit enter.

# to access webapp
# http://localhost:1234 

# to publish on dockerhub
# docker tag atlas_demo christopherpickering/rmc-atlas-demo 
# docker push christopherpickering/rmc-atlas-demo 

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

FROM ubuntu:16.04

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ACCEPT_EULA=Y \
    SA_PASSWORD=p@ssw0rd \
    DEBIAN_FRONTEND=noninteractive \
    MSSQL_PID=Developer

RUN apt-get update || true && \
    apt-get -yq --no-install-recommends install \
        wget \
        apt-utils \
        curl \
        apt-transport-https \
        ca-certificates \
        || true && \
    # install dotnet
    wget https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update || true && \
    apt-get install -yq dotnet-sdk-2.2 aspnetcore-runtime-2.2 && \
    # install full text index
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/16.04/mssql-server-2017.list | tee /etc/apt/sources.list.d/mssql-server.list && \
    apt-get update || true && \
    curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | tee /etc/apt/sources.list.d/msprod.list && \
    apt-get update || true && \
    apt-get install -yq \
        mssql-server \
        mssql-server-ha \
        mssql-server-fts \
        mssql-tools \
        unixodbc-dev \
        || true && \
    # clean up
    apt-get remove -yq --auto-remove \
        curl \
        wget \
        apt-utils \
        apt-transport-https \
        ca-certificates && \
    apt-get clean || true && \
    rm -rf /var/lib/apt/lists

WORKDIR /app

# copy and execute database creation and seed scriptsnot
COPY ["./Data Governance WebApp/DatabaseCreationScript.sql", "create.sql"]
COPY ["./Data Governance WebApp/DatabaseSeedScript.sql", "seed.sql"]

# sql server takes about 30 seconds to start.
RUN /opt/mssql/bin/sqlservr & sleep 30 && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'p@ssw0rd' -d master -i "create.sql" && echo "Create Complete" && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'p@ssw0rd' -d master -i "seed.sql"

COPY --from=build ["/app/Data Governance WebApp/out", "./"]

CMD /opt/mssql/bin/sqlservr & sleep 30 && ASPNETCORE_URLS=http://*:$PORT dotnet "Data Governance WebApp.dll" 