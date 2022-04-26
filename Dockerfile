# to build
# docker build  --tag atlas_demo . --build-arg HOST=host PASSWORD=password USER=user

# to run locally
# docker run -i -t -p 1234:1234 -p 8983:8983 -e PORT=1234  -u 0 atlas_demo:latest

# to get in shell
# docker run --entrypoint sh -i -t -u 0 atlas_demo:latest

# to access webapp
# http://localhost:1234

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app
COPY web.sln .
COPY ["./web/web.csproj", "./web/"]
RUN dotnet restore ./web/web.csproj

COPY ["./web/.", "./web/"]
WORKDIR "/app/web/"
# add analytics
RUN  sed -i -e 's/<\/body>/<script async defer data-website-id="833156f8-3343-4da3-b7d5-45b5fa4f224d" src="https:\/\/analytics.atlas.bi\/umami.js"><\/script><\/body>/g' Pages/Shared/_Layout.cshtml

ARG USER
ARG PASSWORD
ARG HOST

# create config
RUN echo "{\"solr\": {\"atlas_address\": \"https://atlas-dotnet-search.herokuapp.com/solr/atlas\", \"atlas_lookups_address\": \"http://atlas-dotnet-search.herokuapp.com/solr/atlas_lookups\"},\"ConnectionStrings\": {\"AtlasDatabase\": \"Server=$HOST;Database=atlas;User Id=$USER; Password=$PASSWORD; MultipleActiveResultSets=true\"}}" > appsettings.cust.json

# migrate
RUN dotnet tool install --global dotnet-ef \
  && export PATH="$PATH:/root/.dotnet/tools"

RUN dotnet tool restore && dotnet ef database update --project web.csproj -v

RUN dotnet publish -c Release -o out web.csproj

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine
WORKDIR /app
COPY --from=build ["/app/web/out", "./"]

CMD ASPNETCORE_URLS=http://*:$PORT dotnet "Atlas_Web.dll"
