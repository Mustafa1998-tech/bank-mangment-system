# ==============================================
# Bank Management System - Makefile
# ==============================================

.PHONY: help dev prod test clean build deploy backup restore

# Default target
.DEFAULT_GOAL := help

# Colors for output
RED=\033[0;31m
GREEN=\033[0;32m
YELLOW=\033[1;33m
BLUE=\033[0;34m
NC=\033[0m # No Color

# Project variables
PROJECT_NAME=bank-management-system
COMPOSE_FILE=docker-compose.yml
COMPOSE_PROD_FILE=docker-compose.prod.yml
BACKUP_DIR=./backup
TIMESTAMP=$(shell date +%Y%m%d_%H%M%S)

help: ## Show this help message
	@echo "$(BLUE)Bank Management System - Available Commands$(NC)"
	@echo "=============================================="
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "$(GREEN)%-20s$(NC) %s\n", $$1, $$2}' $(MAKEFILE_LIST)

# ==============================================
# Development Commands
# ==============================================

dev: ## Start development environment
	@echo "$(YELLOW)Starting development environment...$(NC)"
	docker-compose up -d
	@echo "$(GREEN)Development environment started!$(NC)"
	@echo "Frontend: http://localhost:3000"
	@echo "Backend API: http://localhost:5000"
	@echo "API Docs: http://localhost:5000/swagger"

dev-build: ## Build and start development environment
	@echo "$(YELLOW)Building and starting development environment...$(NC)"
	docker-compose up -d --build
	@echo "$(GREEN)Development environment built and started!$(NC)"

dev-logs: ## Show development logs
	docker-compose logs -f

dev-stop: ## Stop development environment
	@echo "$(YELLOW)Stopping development environment...$(NC)"
	docker-compose down
	@echo "$(GREEN)Development environment stopped!$(NC)"

dev-clean: ## Clean development environment (removes volumes)
	@echo "$(RED)Cleaning development environment (this will remove all data)...$(NC)"
	@read -p "Are you sure? [y/N] " -n 1 -r; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker-compose down -v --remove-orphans; \
		echo "$(GREEN)Development environment cleaned!$(NC)"; \
	else \
		echo "$(YELLOW)Operation cancelled.$(NC)"; \
	fi

# ==============================================
# Production Commands
# ==============================================

prod: ## Start production environment
	@echo "$(YELLOW)Starting production environment...$(NC)"
	docker-compose -f $(COMPOSE_PROD_FILE) up -d
	@echo "$(GREEN)Production environment started!$(NC)"

prod-build: ## Build and start production environment
	@echo "$(YELLOW)Building and starting production environment...$(NC)"
	docker-compose -f $(COMPOSE_PROD_FILE) up -d --build
	@echo "$(GREEN)Production environment built and started!$(NC)"

prod-logs: ## Show production logs
	docker-compose -f $(COMPOSE_PROD_FILE) logs -f

prod-stop: ## Stop production environment
	@echo "$(YELLOW)Stopping production environment...$(NC)"
	docker-compose -f $(COMPOSE_PROD_FILE) down
	@echo "$(GREEN)Production environment stopped!$(NC)"

# ==============================================
# Build Commands
# ==============================================

build: ## Build all components
	@echo "$(YELLOW)Building all components...$(NC)"
	$(MAKE) build-backend
	$(MAKE) build-frontend
	@echo "$(GREEN)All components built successfully!$(NC)"

build-backend: ## Build backend only
	@echo "$(YELLOW)Building backend...$(NC)"
	cd backend/BankManagement.API && dotnet restore && dotnet build -c Release
	@echo "$(GREEN)Backend built successfully!$(NC)"

build-frontend: ## Build frontend only
	@echo "$(YELLOW)Building frontend...$(NC)"
	cd frontend/web && npm install && npm run build
	@echo "$(GREEN)Frontend built successfully!$(NC)"

# ==============================================
# Test Commands
# ==============================================

test: ## Run all tests
	@echo "$(YELLOW)Running all tests...$(NC)"
	$(MAKE) test-backend
	$(MAKE) test-frontend
	@echo "$(GREEN)All tests completed!$(NC)"

test-backend: ## Run backend tests
	@echo "$(YELLOW)Running backend tests...$(NC)"
	cd backend/BankManagement.Tests && dotnet test --logger "console;verbosity=detailed"

test-frontend: ## Run frontend tests
	@echo "$(YELLOW)Running frontend tests...$(NC)"
	cd frontend/web && npm test -- --coverage --watchAll=false

# ==============================================
# Database Commands
# ==============================================

db-migrate: ## Run database migrations
	@echo "$(YELLOW)Running database migrations...$(NC)"
	cd backend/BankManagement.API && dotnet ef database update
	@echo "$(GREEN)Database migrations completed!$(NC)"

db-seed: ## Seed database with sample data
	@echo "$(YELLOW)Seeding database...$(NC)"
	docker-compose exec backend dotnet run --seed-data
	@echo "$(GREEN)Database seeded successfully!$(NC)"

db-backup: ## Backup database
	@echo "$(YELLOW)Creating database backup...$(NC)"
	@mkdir -p $(BACKUP_DIR)
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U sa -P BankSystem@2024 \
		-Q "BACKUP DATABASE BankManagementDB TO DISK = '/var/opt/mssql/backup/bank_backup_$(TIMESTAMP).bak'"
	docker cp bank-sqlserver:/var/opt/mssql/backup/bank_backup_$(TIMESTAMP).bak $(BACKUP_DIR)/
	@echo "$(GREEN)Database backup created: $(BACKUP_DIR)/bank_backup_$(TIMESTAMP).bak$(NC)"

db-restore: ## Restore database from backup (usage: make db-restore BACKUP_FILE=backup.bak)
	@if [ -z "$(BACKUP_FILE)" ]; then \
		echo "$(RED)Error: Please specify BACKUP_FILE$(NC)"; \
		echo "Usage: make db-restore BACKUP_FILE=backup.bak"; \
		exit 1; \
	fi
	@echo "$(YELLOW)Restoring database from $(BACKUP_FILE)...$(NC)"
	docker cp $(BACKUP_DIR)/$(BACKUP_FILE) bank-sqlserver:/var/opt/mssql/backup/
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
		-S localhost -U sa -P BankSystem@2024 \
		-Q "RESTORE DATABASE BankManagementDB FROM DISK = '/var/opt/mssql/backup/$(BACKUP_FILE)' WITH REPLACE"
	@echo "$(GREEN)Database restored successfully!$(NC)"

# ==============================================
# Monitoring Commands
# ==============================================

status: ## Show services status
	@echo "$(BLUE)Services Status:$(NC)"
	docker-compose ps

health: ## Check services health
	@echo "$(YELLOW)Checking services health...$(NC)"
	@echo "Backend Health:"
	@curl -f http://localhost:5000/health 2>/dev/null && echo " ✅ Backend is healthy" || echo " ❌ Backend is unhealthy"
	@echo "Frontend Health:"
	@curl -f http://localhost:3000/health 2>/dev/null && echo " ✅ Frontend is healthy" || echo " ❌ Frontend is unhealthy"

logs: ## Show all logs
	docker-compose logs -f

logs-backend: ## Show backend logs
	docker-compose logs -f backend

logs-frontend: ## Show frontend logs
	docker-compose logs -f frontend

logs-db: ## Show database logs
	docker-compose logs -f sqlserver

monitor: ## Monitor resource usage
	docker stats

# ==============================================
# Maintenance Commands
# ==============================================

clean: ## Clean Docker resources
	@echo "$(YELLOW)Cleaning Docker resources...$(NC)"
	docker image prune -f
	docker volume prune -f
	@echo "$(GREEN)Docker resources cleaned!$(NC)"

clean-all: ## Clean all Docker resources (WARNING: removes everything)
	@echo "$(RED)This will remove ALL Docker resources including other projects!$(NC)"
	@read -p "Are you sure? [y/N] " -n 1 -r; \
	if [[ $$REPLY =~ ^[Yy]$$ ]]; then \
		docker system prune -af --volumes; \
		echo "$(GREEN)All Docker resources cleaned!$(NC)"; \
	else \
		echo "$(YELLOW)Operation cancelled.$(NC)"; \
	fi

update: ## Update project from git and rebuild
	@echo "$(YELLOW)Updating project...$(NC)"
	git pull
	$(MAKE) dev-build
	@echo "$(GREEN)Project updated successfully!$(NC)"

# ==============================================
# Setup Commands
# ==============================================

setup: ## Initial project setup
	@echo "$(YELLOW)Setting up project...$(NC)"
	@echo "Checking prerequisites..."
	@command -v docker >/dev/null 2>&1 || { echo "$(RED)Docker is required but not installed.$(NC)"; exit 1; }
	@command -v docker-compose >/dev/null 2>&1 || { echo "$(RED)Docker Compose is required but not installed.$(NC)"; exit 1; }
	@echo "Prerequisites check passed ✅"
	@echo "Creating necessary directories..."
	@mkdir -p logs backup nginx/ssl
	@echo "Copying environment file..."
	@cp .env.example .env 2>/dev/null || echo ".env already exists"
	@echo "$(GREEN)Project setup completed!$(NC)"
	@echo "$(BLUE)Next steps:$(NC)"
	@echo "1. Review and update .env file"
	@echo "2. Run 'make dev' to start development environment"

ssl: ## Generate SSL certificates for development
	@echo "$(YELLOW)Generating SSL certificates...$(NC)"
	@mkdir -p nginx/ssl
	openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
		-keyout nginx/ssl/nginx.key \
		-out nginx/ssl/nginx.crt \
		-subj "/C=SA/ST=Riyadh/L=Riyadh/O=Bank Management/CN=localhost"
	@echo "$(GREEN)SSL certificates generated!$(NC)"

# ==============================================
# Documentation Commands
# ==============================================

docs: ## Open API documentation
	@echo "$(BLUE)Opening API documentation...$(NC)"
	@echo "API Documentation: http://localhost:5000/swagger"
	@command -v xdg-open >/dev/null 2>&1 && xdg-open http://localhost:5000/swagger || \
	command -v open >/dev/null 2>&1 && open http://localhost:5000/swagger || \
	echo "Please open http://localhost:5000/swagger in your browser"

# ==============================================
# Utility Commands
# ==============================================

shell-backend: ## Open shell in backend container
	docker-compose exec backend /bin/bash

shell-frontend: ## Open shell in frontend container
	docker-compose exec frontend /bin/sh

shell-db: ## Open SQL Server shell
	docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P BankSystem@2024

# ==============================================
# Quick Start Commands
# ==============================================

quick-start: ## Quick start for new users
	@echo "$(BLUE)🚀 Bank Management System - Quick Start$(NC)"
	@echo "========================================"
	$(MAKE) setup
	$(MAKE) dev
	@echo "$(GREEN)🎉 System is ready!$(NC)"
	@echo "$(BLUE)Access URLs:$(NC)"
	@echo "• Frontend: http://localhost:3000"
	@echo "• Backend API: http://localhost:5000"
	@echo "• API Documentation: http://localhost:5000/swagger"

# ==============================================
# Info Commands
# ==============================================

info: ## Show project information
	@echo "$(BLUE)Bank Management System Information$(NC)"
	@echo "=================================="
	@echo "Project: $(PROJECT_NAME)"
	@echo "Version: 1.0.0"
	@echo "Backend: .NET 8 + SQL Server"
	@echo "Frontend: React + TypeScript"
	@echo "Infrastructure: Docker + Docker Compose"
	@echo ""
	@echo "$(BLUE)Useful Commands:$(NC)"
	@echo "• make quick-start  - Complete setup and start"
	@echo "• make dev         - Start development environment"
	@echo "• make test        - Run all tests"
	@echo "• make help        - Show all available commands"

