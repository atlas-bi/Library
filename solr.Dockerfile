# to build
# docker build  --build-arg HOST=host PASSWORD=password USER=user --tag atlas_demo_search -f solr.Dockerfile .

# to run locally
# docker run -i -t -p 8983:8983 -e PORT=8983 -u 0 atlas_demo_search:latest

# to get in shell
# docker run --entrypoint sh -i -t -u 0 atlas_demo_search:latest

# to access webapp
# http://localhost:8983


FROM python:3.14-alpine AS builder

WORKDIR /build

RUN apk add --no-cache \
    gcc g++ libc-dev python3-dev \
    unixodbc-dev libffi-dev libxml2-dev \
    git curl bash

RUN pip install --no-cache-dir \
    pyodbc pysolr pytz python-dotenv

RUN git clone --depth=1 https://github.com/atlas-bi/Solr-Search-ETL.git etl && \
    rm -rf etl/.git



FROM python:3.14-alpine
WORKDIR /app

RUN apk add --no-cache \
    openjdk11-jre \
    unixodbc \
    bash curl

RUN curl -O https://download.microsoft.com/download/b/9/f/b9f3cce4-3925-46d4-9f46-da08869c6486/msodbcsql18_18.1.1.1-1_amd64.apk && \
    apk add --allow-untrusted msodbcsql18_18.1.1.1-1_amd64.apk && \
    rm msodbcsql18_18.1.1.1-1_amd64.apk


COPY --from=builder /usr/lib/python3.14/site-packages /usr/lib/python3.14/site-packages
COPY --from=builder /build/etl /app/etl

COPY ./web/solr /app

COPY scripts/solr_start.sh /start.sh
RUN chmod +x /start.sh

RUN addgroup -S app && adduser -S app -G app && \
    chown -R app:app /app /start.sh

USER app

CMD ["/start.sh"]

