# Receivables API

API para gerenciamento de recebíveis e antecipação de notas fiscais.

##  Como Rodar o Projeto

### Pré-requisitos
- Docker e Docker Compose
- .NET 9.0 SDK (para desenvolvimento local)

## Iniciar o Projeto

```bash
# 1. Subir o banco de dados
docker-compose up -d

# 2. Rodar a API
cd receivables
dotnet run
```

Acesse: **http://localhost:5000/swagger** 

As migrations são aplicadas automaticamente!

---

## Comandos Úteis

```bash
# Docker
docker-compose up -d          # Subir banco
docker-compose down           # Parar banco
docker-compose logs -f        # Ver logs
docker-compose down -v        # Resetar banco

# Testes
dotnet test
