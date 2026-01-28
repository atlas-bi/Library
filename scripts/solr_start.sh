#!/bin/bash
set -e

echo "Starting startup script..."

PORT=${PORT:-8983}

cd /app/etl

echo "Generating .env..."
echo "Debug: HOST=$HOST USER=$USER"

export SOLRURL="http://localhost:$PORT/solr/atlas"
export SOLRLOOKUPURL="http://localhost:$PORT/solr/atlas_lookups"

echo "SOLRURL = \"$SOLRURL\"" > .env
echo "SOLRLOOKUPURL = \"$SOLRLOOKUPURL\"" >> .env

export ATLASDATABASE="DRIVER={ODBC Driver 18 for SQL Server};SERVER=$HOST;DATABASE=atlas;UID=$USER;PWD=$PASSWORD;TrustServerCertificate=YES;LoginTimeout=60"
echo "ATLASDATABASE = \"$ATLASDATABASE\"" >> .env

cd /app

echo "Starting Solr (foreground mode, managed by container)..."
bin/solr start -force -noprompt -f -p $PORT &
SOLR_PID=$!

echo "Solr PID: $SOLR_PID"
echo "Waiting for Solr..."
sleep 15

echo "Checking SQL Server connectivity..."
set +e

python3 -u - <<'EOF'
import pyodbc, os, time

conn_str = os.environ.get("ATLASDATABASE")
if not conn_str:
    print("Error: ATLASDATABASE env var is missing")
    exit(1)

print("Attempting connection to SQL Server...")
for i in range(30):
    try:
        pyodbc.connect(conn_str)
        print("Successfully connected to SQL Server")
        exit(0)
    except Exception as e:
        print(f"Connection attempt {i+1} failed: {e}")
        time.sleep(5)

print("Could not connect to SQL Server after retries")
exit(1)
EOF

DB_CHECK_EXIT=$?
set -e

if [ $DB_CHECK_EXIT -eq 0 ]; then
    echo "Running ETL..."
    cd /app/etl
    python3 -u atlas_collections.py || echo "Failed atlas_collections.py"
    python3 -u atlas_groups.py || echo "Failed atlas_groups.py"
    python3 -u atlas_initiatives.py || echo "Failed atlas_initiatives.py"
    python3 -u atlas_lookups.py || echo "Failed atlas_lookups.py"
    python3 -u atlas_reports.py || echo "Failed atlas_reports.py"
    python3 -u atlas_terms.py || echo "Failed atlas_terms.py"
    python3 -u atlas_users.py || echo "Failed atlas_users.py"
else
    echo "Skipping ETL due to DB connection failure"
fi

cd /app

echo "Solr is running; waiting on PID $SOLR_PID..."
wait $SOLR_PID
