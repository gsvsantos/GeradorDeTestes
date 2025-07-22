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
3. Defina o projeto `GeradorDeTestes.WebApp` como **Startup Project**.
4. Execute a aplicação (`F5` ou `Ctrl + F5`).

## Projetos da Solução

- **GeradorDeTestes.WebApp** — Interface web (ASP.NET Core MVC)
- **GeradorDeTestes.Dominio** — Entidades, regras de negócio, contratos
- **GeradorDeTestes.Infraestrutura** — Persistência de dados

## Requisitos

- .NET 8 SDK
- Visual Studio 2022+

---

_Readme inicial — detalhes técnicos e imagens das telas serão adicionados futuramente._
