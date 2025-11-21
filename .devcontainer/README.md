# Dev Container Setup

This dev container provides a complete development environment for the LookupApi project with MS SQL Server.

## What's Included

- **.NET 10.0 SDK** - For building and running the application
- **MS SQL Server 2022** - Database server running in a separate container
- **SQL Server Tools** - Command-line tools for database management
- **VS Code Extensions**:
  - C# Dev Kit
  - SQL Server (mssql)
  - Docker
  - REST Client

## Getting Started

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Install [VS Code](https://code.visualstudio.com/)
3. Install the [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
4. Open this project in VS Code
5. When prompted, click "Reopen in Container" (or use Command Palette: "Dev Containers: Reopen in Container")

## Database Connection

The SQL Server instance is automatically configured with:

- **Server**: `mssql` (or `localhost` from your host machine)
- **Port**: `1433`
- **Database**: `LookupDb`
- **Username**: `sa`
- **Password**: `YourStrong@Passw0rd`

### Connection String Example

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=mssql;Database=LookupDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

## Running the Application

```bash
cd src
dotnet run
```

The API will be available at `http://localhost:5000`

## Database Management

### Using SQL Server Extension in VS Code

The SQL Server extension is pre-configured with a connection. Click on the SQL Server icon in the sidebar to connect and run queries.

### Using Command Line

```bash
# Connect to SQL Server
/opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C

# Run a query
/opt/mssql-tools18/bin/sqlcmd -S mssql -U sa -P YourStrong@Passw0rd -C -d LookupDb -Q "SELECT * FROM sys.tables"
```

### Database Initialization

The `init-db.sql` script runs automatically when the container is first created. Modify it to add your own tables and seed data.

## Customization

### Change SQL Server Password

1. Update the password in `.devcontainer/docker-compose.yml`
2. Update the password in `.devcontainer/devcontainer.json` (mssql.connections)
3. Update the password in `.devcontainer/wait-for-sql.sh`
4. Rebuild the container

### Add Database Migrations

Place your SQL migration scripts in the `.devcontainer` folder and update `init-db.sql` to include them.

## Troubleshooting

### SQL Server Not Ready

If you see connection errors, wait a moment for SQL Server to fully start. The container includes a health check that waits for SQL Server to be ready.

### Database Not Created

Check the logs:
```bash
docker logs <container-name>
```

### Reset Everything

To start fresh:
1. Close VS Code
2. Run: `docker-compose -f .devcontainer/docker-compose.yml down -v`
3. Reopen in container

## Notes

- The SQL Server data is persisted in a Docker volume named `mssql-data`
- The container uses SQL Server 2022 Developer Edition (free for development)
- TrustServerCertificate=True is used for development; use proper certificates in production
