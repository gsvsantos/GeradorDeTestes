# 🧠 Teste, Logo Avalia 🧠

![](https://i.pinimg.com/736x/cc/ae/26/ccae26e01396978af1a57a6de1045c45.jpg)

## Introdução
Teste, Logo Avalia é um sistema modular para criação, gerenciamento e distribuição de provas escolares. Projetado com foco em clareza, automação e integridade pedagógica, oferece suporte completo ao ciclo de avaliação: desde o cadastro de disciplinas até a geração de PDFs de testes e gabaritos — tudo isso com um toque filosófico e precisão algorítmica.

> Porque em tempos de ensino moderno... se pode testar, pode avaliar.

***

## 🧩 Módulos e Funcionalidades

### 📚 *Módulo de Disciplina*
- **Cadastro de novas disciplinas**
  - Campo nome obrigatório
  - Proíbe duplicações
  - Armazena vínculo com matérias
- **Edição de disciplinas**
  - Mesmos critérios do cadastro
- **Exclusão de disciplinas**
  - Impede exclusão se houver matérias ou testes vinculados
- **Visualização de disciplinas**
  - Exibe: Id e Nome
<br><br>

### 📝 *Módulo de Matéria*
- **Cadastro de novas matérias**
  - Campos: Nome, Disciplina, Série (todos obrigatórios)
  - Proíbe duplicações
- **Edição de matérias**
  - Mesmos critérios do cadastro
- **Exclusão de matérias**
  - Impede exclusão se houver questões vinculadas
- **Visualização de matérias**
  - Exibe: Id, Nome, Disciplina, Série e Questões vinculadas
<br><br>

### ❓ *Módulo de Questões*
- **Cadastro de novas questões**
  - Campos: Matéria, Enunciado, Alternativas (todos obrigatórios)
  - Requer de 2 a 4 alternativas por questão
- **Edição de questões**
  - Mesmos critérios do cadastro
- **Exclusão de questões**
  - Impede remoção se estiverem em testes
- **Visualização de questões**
  - Exibe: Id, Enunciado, Matéria, Resposta Correta
- **Configuração de alternativas**
  - Permite adicionar/remover alternativas
  - Obrigatório ter exatamente uma alternativa correta
  - Impede mais de uma correta e menos de duas alternativas
<br><br>

### 🧾 *Módulo de Testes*
- **Geração de Testes**
  - Campos: Título, Disciplina, Matéria*, Série, Quantidade de Questões (todos obrigatórios)
  - Evita duplicação de títulos
  - Questões selecionadas aleatoriamente
  - Matérias são carregadas com base na disciplina selecionada
  - *Prova de Recuperação*: considera todas as matérias da disciplina
- **Duplicação de testes**
  - Copia dados principais (exceto questões)
  - Evita duplicações de título
- **Exclusão de testes**
  - Permite apagar testes existentes
- **Visualização de geral**
  - Exibe: Id, Título, Disciplina, Matéria (ou "Recuperação"), Quantidade de Questões
- **Visualização detalhada**
  - Mostra todas as informações de um teste, incluindo suas questões
- **Geração de PDF**
  - Teste: Título, Disciplina, Matéria, Questões e Alternativas
  - Gabarito: Mesmo conteúdo, com alternativa correta destacada
<br><br>

![](https://images-wixmp-ed30a86b8c4ca887773594c2.wixmp.com/f/64de309d-e159-4d7d-923a-55d2db70c43e/dgh2oh7-e18892a0-9d29-4b88-87c0-429a4b82a5fd.gif?token=eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1cm46YXBwOjdlMGQxODg5ODIyNjQzNzNhNWYwZDQxNWVhMGQyNmUwIiwiaXNzIjoidXJuOmFwcDo3ZTBkMTg4OTgyMjY0MzczYTVmMGQ0MTVlYTBkMjZlMCIsIm9iaiI6W1t7InBhdGgiOiJcL2ZcLzY0ZGUzMDlkLWUxNTktNGQ3ZC05MjNhLTU1ZDJkYjcwYzQzZVwvZGdoMm9oNy1lMTg4OTJhMC05ZDI5LTRiODgtODdjMC00MjlhNGI4MmE1ZmQuZ2lmIn1dXSwiYXVkIjpbInVybjpzZXJ2aWNlOmZpbGUuZG93bmxvYWQiXX0.Dg1fOakEBy1W4STJ0I-TjgXVC2p2wpHhnEoZwRanhQI) 
***

## Tecnologias
![Tecnologias](https://skillicons.dev/icons?i=github,visualstudio,vscode,cs,dotnet,html,css,javascript,bootstrap)

***

## Como utilizar
1. Clone o repositório ou baixe o código fonte.

```
git clone https://github.com/Compila-logo-existe/GeradorDeTestes
```

2. Acesse a pasta do projeto:
   
```
cd GeradorDeTestes.WebApp
```

3. Restaure as dependências:
   
```
dotnet restore
```

4. Compile a aplicação:
   
```
dotnet build --configuration Release
```

5. Execute o projeto:
   
```
dotnet run --project GeradorDeTestes.WebApp
```

#### Após executar, procure pelo link local. Exemplos: [https://localhost:0000 | http://localhost:0000]
  
## Requisitos

- .NET SDK 8.0 ou superior

- Navegador moderno (Edge, Chrome, Firefox etc.)

- Editor recomendado: Visual Studio 2022 ou superior (com suporte a ASP.NET MVC)

# Filosofia de Projeto
"Toda disciplina merece ser registrada.

Toda matéria precisa de contexto.

Toda questão deve ser formulada com propósito.

E todo teste... deve existir para avaliar."

— Compila, Logo Existe

![](https://pt.quizur.com/_image?href=https://img.quizur.com/f/img5fc6ad8c1078e7.73118609.jpeg?lastEdited=1606856087&w=600&h=600&f=webp) 
> Imagens reais de René Descartes utilizando pdfs gerados pelo site."
