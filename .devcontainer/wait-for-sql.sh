#!/bin/bash

echo "Waiting for SQL Server to be ready..."

# Wait for SQL Server to be ready
for i in {1..30}; do
    if /opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C -Q "SELECT 1" > /dev/null 2>&1; then
        echo "SQL Server is ready!"
        
        # Check if database exists, if not create it
        DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C -Q "SELECT name FROM sys.databases WHERE name = 'LookupDb'" -h -1 2>/dev/null | grep -c "LookupDb")
        
        if [ "$DB_EXISTS" -eq "0" ]; then
            echo "Creating LookupDb database..."
            /opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C -Q "CREATE DATABASE LookupDb"
            
            # Run initialization script if it exists
            if [ -f ".devcontainer/init-db.sql" ]; then
                echo "Running database initialization script..."
                /opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C -d LookupDb -i .devcontainer/init-db.sql
            fi
            
            echo "Database setup complete!"
        else
            echo "Database LookupDb already exists."
        fi
        
        exit 0
    fi
    echo "Attempt $i: SQL Server not ready yet, waiting..."
    sleep 2
done

echo "SQL Server failed to become ready in time"
exit 1
