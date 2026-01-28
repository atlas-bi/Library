# to build
# docker build --build-arg HOST=host --build-arg PASSWORD=password --build-arg USER=user -t atlas_demo_search -f solr.Dockerfile .
# docker buildx build --platform linux/amd64 -t atlas_demo_search -f solr.Dockerfile .

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
    bash curl ca-certificates unixodbc openjdk11-jre \
    && update-ca-certificates

RUN ARCH=$(case $(uname -m) in \
        x86_64) echo amd64 ;; \
        arm64) echo arm64 ;; \
        aarch64) echo arm64 ;; \
        *) echo unsupported ;; \
    esac) && \
    if [ "$ARCH" = "unsupported" ]; then echo "unsupported architecture"; exit 1; fi && \
    curl -O -k https://download.microsoft.com/download/9dcab408-e0d4-4571-a81a-5a0951e3445f/msodbcsql18_18.6.1.1-1_${ARCH}.apk && \
    apk add --allow-untrusted msodbcsql18_18.6.1.1-1_${ARCH}.apk && \
    rm msodbcsql18_18.6.1.1-1_${ARCH}.apk

COPY --from=builder /usr/local/lib/python3.14/site-packages /usr/local/lib/python3.14/site-packages
COPY --from=builder /build/etl /app/etl

COPY ./web/solr /app

COPY scripts/solr_start.sh /start.sh
RUN chmod +x /start.sh

RUN addgroup -S app && adduser -S app -G app && \
    chown -R app:app /app /start.sh

USER app

CMD ["/start.sh"]

