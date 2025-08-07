# KRTBank API

## Visão Geral

A **KRTBank API** é uma solução de backend desenvolvida para o time de Onboarding do banco KRT. Ela fornece uma API RESTful para gerenciar contas de clientes, implementando funcionalidades completas de CRUD (Criar, Ler, Atualizar, Deletar). A API garante que outras áreas do banco (ex.: prevenção a fraudes e emissão de cartões) sejam notificadas sobre alterações nas contas por meio de eventos de domínio. Além disso, otimiza consultas ao banco de dados para reduzir custos na AWS usando cache em memória.

Este projeto foi construído seguindo os princípios de Clean Code, SOLID, Domain-Driven Design (DDD) e incluindo testes unitários.

## Funcionalidades

- **Gerenciamento de Contas (CRUD)**:
  - Criar contas com nome do titular, CPF e status (Ativa/Inativa).
  - Listar todas as contas ou buscar por ID.
  - Atualizar dados da conta, incluindo CPF.
  - Deletar contas.
- **Notificações de Eventos**:
  - Publica eventos de domínio (`AccountCreatedEvent`, `AccountUpdatedEvent`, `AccountDeletedEvent`) para notificar áreas como prevenção a fraudes e emissão de cartões.
- **Otimização de Custos**:
  - Utiliza cache em memória para reduzir consultas repetidas ao banco no mesmo dia.
- **Validação**:
  - Valida CPF (11 dígitos numéricos).
- **Documentação da API**:
  - Interface Swagger para teste interativo dos endpoints.

## Tecnologias

- **.NET 9**: Framework backend.
- **SQLite**: Banco de dados para armazenar dados das contas.
- **Entity Framework Core**: ORM para operações no banco.
- **MediatR**: Para publicação de eventos de domínio.
- **Microsoft.Extensions.Caching.Memory**: Para cache em memória.
- **xUnit e Moq**: Para testes unitários.
- **ASP.NET Core MVC**: Para estrutura da API RESTful.
- **Clean Code, SOLID, DDD**: Princípios de design para código manutenível e escalável.

## Estrutura do Projeto

```
KRTBank/
├── KRTBank.API/                # API Web ASP.NET Core (Controladores, Program.cs)
├── KRTBank.Application/        # Camada de aplicação (Serviços, DTOs)
├── KRTBank.Domain/             # Camada de domínio (Entidades, Interfaces, Eventos)
├── KRTBank.Infrastructure/     # Camada de infraestrutura (Repositórios, Serviços, DB)
├── KRTBank.Tests/             # Testes unitários (xUnit, Moq)
├── krtbank.db                 # Banco SQLite (excluído do Git)
└── README.md                  # Documentação do projeto
```

## Instruções de Configuração

### Instalação
1. **Clonar o repositório**:
   ```bash
   git clone https://github.com/dequim1000/KRTBank.git
   cd KRTBank
   ```

2. **Restaurar dependências**:
   ```bash
   dotnet restore
   ```

3. **Aplicar migrações do banco**:
   ```bash
   dotnet ef database update --project KRTBank.Infrastructure --startup-project KRTBank.API
   ```

4. **Executar a API**:
   ```bash
   cd KRTBank.API
   dotnet run --launch-profile https
   ```

5. **Acessar a API**:
   - Abra `https://localhost:7256/swagger` no navegador para testar os endpoints com o Swagger.

### Endpoints
- **GET /api/Accounts**: Lista todas as contas.
- **GET /api/Accounts/{id}**: Busca uma conta por ID.
- **POST /api/Accounts**: Cria uma nova conta.
  ```json
  {
      "holderName": "André Otávio",
      "cpf": "12345678901"
  }
  ```
- **PUT /api/Accounts/{id}**: Atualiza uma conta. (O CPF não vai atualizar por regra de não alterar o CPF do cadastro)
  ```json
  {
      "id": "guid",
      "holderName": "Mario Kart",
      "cpf": "12345678901",
      "status": "Active"
  }
  ```
- **DELETE /api/Accounts/{id}**: Deleta uma conta.

## Executando Testes

1. **Garantir dependências restauradas**:
   ```bash
   dotnet restore
   ```

2. **Executar testes unitários**:
   ```bash
   dotnet test
   ```

## Cobertura de Testes
- **AccountServiceTests**: Testa operações CRUD e publicação de eventos.

## Observações
- O projeto usa cache em memória (`IMemoryCache`) com expiração de 1 dia para reduzir consultas ao banco, atendendo à preocupação com custos na AWS.
- Eventos de domínio são publicados via MediatR para notificar áreas de prevenção a fraudes e emissão de cartões.
- O arquivo `.gitignore` exclui `krtbank.db`, `bin` e `obj` para evitar commits desnecessários.
- A validação do CPF garante 11 dígitos numéricos;
- Para montar a estrutura e os testes utilizei IA para facilitar a criação
- Os testes ele tem algumas melhorias para funcionar 100%
- Em uma melhoria podemos implementar o RabbitMQ
