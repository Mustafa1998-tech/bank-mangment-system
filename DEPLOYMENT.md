# Bank Management System - Deployment Guide

This guide provides step-by-step instructions for deploying the Bank Management System to a production environment.

## Prerequisites

- Docker and Docker Compose installed on the deployment server
- Git installed for version control
- Access to a container registry (if using private images)
- Domain name and SSL certificates (for production)

## Environment Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/bank-management-system.git
   cd bank-management-system
   ```

2. Copy the example environment file and update with your production values:
   ```bash
   cp .env.example .env
   ```

3. Update the following environment variables in `.env`:
   - Database credentials
   - JWT secret key (generate a strong one)
   - Encryption key (32-byte key)
   - CORS allowed origins
   - Any other environment-specific settings

## Database Setup

### For Development (SQL Server in Docker)

1. Start the database container:
   ```bash
   docker-compose up -d sqlserver
   ```

2. Run database migrations:
   ```bash
   cd backend/BankManagement.API
   dotnet ef database update
   ```

### For Production (PostgreSQL on Render)

1. Create a new PostgreSQL database on Render
2. Update the connection string in your environment variables:
   ```
   ConnectionStrings__DefaultConnection=Host=your-db-host;Database=your-db-name;Username=your-username;Password=your-password
   ```
3. Run database migrations on application startup (handled automatically by the API)

## Building and Running

### Development Mode

1. Start all services:
   ```bash
   docker-compose up --build
   ```

2. Access the application:
   - API: http://localhost:5000
   - Frontend: http://localhost:3000

### Production Deployment

1. Build and push Docker images:
   ```bash
   docker-compose -f docker-compose.prod.yml build
   docker-compose -f docker-compose.prod.yml push
   ```

2. Deploy to your infrastructure:
   - For Render: Push to your repository connected to Render
   - For other platforms: Use their respective deployment methods

## Environment Variables

| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name (Development/Production) | Yes | Production |
| `ConnectionStrings__DefaultConnection` | Database connection string | Yes | |
| `JWT__SecretKey` | JWT signing key | Yes | |
| `JWT__Issuer` | JWT issuer | Yes | |
| `JWT__Audience` | JWT audience | Yes | |
| `ENCRYPTION_KEY` | 32-byte encryption key | Yes | |
| `CORS__AllowedOrigins` | Semicolon-separated list of allowed origins | Yes | |
| `LOG_LEVEL` | Logging level | No | Information |

## Health Checks

The API includes health check endpoints:

- `GET /health` - Basic health check
- `GET /health/db` - Database health check
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

## Monitoring and Logging

- Application logs are written to `logs/bank-management-{date}.log`
- For production, consider using a centralized logging solution
- Monitor application health using the health check endpoints

## Backup and Recovery

### Database Backups

For production, implement regular database backups:

```bash
# Example backup command for PostgreSQL
pg_dump -h your-db-host -U your-username -d your-db-name > backup_$(date +%Y%m%d).sql
```

### Restoring from Backup

```bash
# Example restore command for PostgreSQL
psql -h your-db-host -U your-username -d your-db-name < backup_file.sql
```

## Security Considerations

- Always use HTTPS in production
- Keep all secrets and API keys in environment variables, never in source control
- Regularly rotate database credentials and API keys
- Implement rate limiting and request validation
- Keep all dependencies up to date
- Follow the principle of least privilege for database users

## Troubleshooting

### Common Issues

1. **Database connection errors**
   - Verify database server is running
   - Check connection string and credentials
   - Ensure network connectivity between services

2. **CORS issues**
   - Verify `CORS__AllowedOrigins` includes your frontend URL
   - Check for typos in origin URLs

3. **JWT validation errors**
   - Ensure `JWT__SecretKey` matches between API instances
   - Verify token expiration and issuer/audience claims

### Checking Logs

View container logs:
```bash
docker-compose logs -f
```

## Upgrading

1. Pull the latest changes:
   ```bash
   git pull origin main
   ```

2. Rebuild and restart containers:
   ```bash
   docker-compose down
   docker-compose up -d --build
   ```

3. Run any new database migrations:
   ```bash
   cd backend/BankManagement.API
   dotnet ef database update
   ```

## Support

For issues and support, please contact the development team or open an issue in the repository.
