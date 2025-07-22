# Gerador de Testes

Uma aplicação web para gerenciamento de testes escolares, com funcionalidades como cadastro de disciplinas, matérias, questões e geração automática de provas, incluindo PDF de teste e gabarito.

## Funcionalidades

- Cadastro de Disciplinas
- Cadastro de Matérias (vinculadas às Disciplinas)
- Cadastro e Edição de Questões com alternativas (mín. 2, máx. 4, uma correta)
- Geração Automática de Testes com seleção aleatória de questões
- Duplicação de Testes
- Exportação de Testes e Gabaritos em PDF

## Como executar

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. Abra a solução `GeradorDeTestes.sln` no Visual Studio 2022 ou superior.
3. Configure a string de conexão usando User Secrets (veja abaixo).
4. Defina o projeto `GeradorDeTestes.WebApp` como **Startup Project**.
5. Execute a aplicação (`F5` ou `Ctrl + F5`).

## Configuração da String de Conexão

Antes de rodar a aplicação, configure a string de conexão usando User Secrets do .NET:

dotnet user-secrets init
dotnet user-secrets set "SQL_CONNECTION_STRING" "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=GeradorDeTestesDb;Integrated Security=True"

✅ Isso armazena a configuração localmente, sem expor senhas no código.

✅ O projeto deve ler essa variável usando:

builder.Configuration["SQL_CONNECTION_STRING"]

ou equivalente no seu Program.cs ou appsettings.json.
## Projetos da Solução

- GeradorDeTestes.WebApp — Interface web (ASP.NET Core MVC)
- GeradorDeTestes.Dominio — Entidades, regras de negócio, contratos
- GeradorDeTestes.Infraestrutura — Persistência de dados

## Requisitos

- .NET 8 SDK
- Visual Studio 2022+

## Documentação Visual e Técnica

### Diagrama Lucidchart

![Diagrama do Gerador de Testes](/docs/Trabalho%20-%20Gerador%20de%20Testes%20-%20Lucid.App.svg)

### Diagrama Excalidraw (editável)

📁 [Abrir diagrama editável no Excalidraw](/docs/Trabalho%20-%20Gerador%20de%20Testes%20-%20Excalidraw.svg)

### Requisitos Detalhados

📄 [Visualizar PDF com requisitos detalhados](/docs/Trabalho%20-%20Gerador%20de%20Testes%20-%20Lucid.App.pdf)

---

_Readme inicial — detalhes técnicos e imagens das telas serão adicionados futuramente._
