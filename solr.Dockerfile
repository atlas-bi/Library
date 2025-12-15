# to build
# docker build  --build-arg HOST=host PASSWORD=password USER=user --tag atlas_demo_search -f solr.Dockerfile .

# to run locally
# docker run -i -t -p 8983:8983 -e PORT=8983 -u 0 atlas_demo_search:latest

# to get in shell
# docker run --entrypoint sh -i -t -u 0 atlas_demo_search:latest

# to access webapp
# http://localhost:8983

FROM python:3.12-alpine as search
WORKDIR /app

# startup search and load data
RUN apk add --no-cache openjdk11 bash lsof python3-dev curl gcc git py3-pip gcc libc-dev g++ libffi-dev libxml2 unixodbc-dev && \
    pip3 install pyodbc pysolr pytz python-dotenv

# install sql server driver
RUN curl -O https://download.microsoft.com/download/b/9/f/b9f3cce4-3925-46d4-9f46-da08869c6486/msodbcsql18_18.1.1.1-1_amd64.apk && \
    apk add --allow-untrusted msodbcsql18_18.1.1.1-1_amd64.apk

# Copy site
COPY ["./web/solr", "./"]

# Pull solr etl
RUN mkdir etl && cd etl && git clone --depth 1 https://github.com/atlas-bi/Solr-Search-ETL.git .

# Permissions
RUN chmod -R 777 bin

# Create startup script
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'set -e' >> /start.sh && \
    echo 'echo "Starting startup script..."' >> /start.sh && \
    echo 'PORT=${PORT:-8983}' >> /start.sh && \
    echo 'cd /app/etl' >> /start.sh && \
    echo 'echo "Generating .env..."' >> /start.sh && \
    echo 'echo "SOLRURL = \"http://localhost:$PORT/solr/atlas\"" > .env' >> /start.sh && \
    echo 'echo "SOLRLOOKUPURL = \"http://localhost:$PORT/solr/atlas_lookups\"" >> .env' >> /start.sh && \
    echo 'echo "ATLASDATABASE = \"DRIVER={ODBC Driver 18 for SQL Server};SERVER=$HOST;DATABASE=atlas;UID=$USER;PWD=$PASSWORD;TrustServerCertificate=YES\"" >> .env' >> /start.sh && \
    echo 'cd /app' >> /start.sh && \
    echo 'echo "Starting Solr in background..."' >> /start.sh && \
    echo 'bin/solr start -force -noprompt -p $PORT' >> /start.sh && \
    echo 'echo "Waiting for Solr..."' >> /start.sh && \
    echo 'sleep 15' >> /start.sh && \
    echo 'echo "Running ETL..."' >> /start.sh && \
    echo 'cd /app/etl' >> /start.sh && \
    echo 'python3 atlas_collections.py || echo "Failed atlas_collections.py"' >> /start.sh && \
    echo 'python3 atlas_groups.py || echo "Failed atlas_groups.py"' >> /start.sh && \
    echo 'python3 atlas_initiatives.py || echo "Failed atlas_initiatives.py"' >> /start.sh && \
    echo 'python3 atlas_lookups.py || echo "Failed atlas_lookups.py"' >> /start.sh && \
    echo 'python3 atlas_reports.py || echo "Failed atlas_reports.py"' >> /start.sh && \
    echo 'python3 atlas_terms.py || echo "Failed atlas_terms.py"' >> /start.sh && \
    echo 'python3 atlas_users.py || echo "Failed atlas_users.py"' >> /start.sh && \
    echo 'cd /app' >> /start.sh && \
    echo 'echo "Restarting Solr in foreground..."' >> /start.sh && \
    echo 'bin/solr stop -p $PORT' >> /start.sh && \
    echo 'exec bin/solr start -force -noprompt -f -p $PORT' >> /start.sh && \
    chmod +x /start.sh

CMD ["/start.sh"]
