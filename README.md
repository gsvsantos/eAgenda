# ☎️​ Prototype E-Agenda ☎️​

![](https://i.pinimg.com/736x/d3/dd/c8/d3ddc80f8836ebcaa8919719c5f5d147.jpg)

## Introdução
A **Prototype E-Agenda** é um sistema eletrônico desenvolvido para organização pessoal e profissional, com foco em eficiência, clareza e confiabilidade. Sua estrutura modular permite o controle completo de compromissos, contatos, tarefas, despesas e categorias, mantendo o padrão visual e funcional das soluções corporativas da década. Projetada para ser prática e robusta, oferece ao usuário uma experiência consistente e intuitiva.

***

## 🧩 Módulos e Funcionalidades

### 📇 *Módulo de Contatos*
- Cadastro, edição, visualização e exclusão de contatos.
- Validações:
  - Nome (2–100 caracteres)
  - Email com formato válido
  - Telefone no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX
- Campos opcionais: Cargo e Empresa
- Evita duplicação de email/telefone.
- Impede exclusão de contatos vinculados a compromissos.
<br><br>

### 📆 *Módulo de Compromissos*
- Inserção, edição, exclusão e visualização de compromissos.
- Campos obrigatórios:
  - Assunto (2–100 caracteres)
  - Data, horário de início e término
  - Tipo: Remoto ou Presencial
  - Link (remoto) ou Local (presencial)
- Contato vinculado (opcional).
- Verificação de conflitos de horário.
<br><br>

### 🗂️ *Módulo de Categorias*
- Cadastro, edição, exclusão e listagem de categorias.
- Visualização de despesas por categoria.
- Validações:
  - Título único (2–100 caracteres)
- Impede exclusão de categorias vinculadas a despesas.
<br><br>

### 💸 *Módulo de Despesas*
- Registro, edição, visualização e exclusão de despesas.
- Campos obrigatórios:
  - Descrição (2–100 caracteres)
  - Valor (em R$)
  - Forma de pagamento (À vista, Crédito, Débito)
  - Uma ou mais categorias
- Data de ocorrência opcional (padrão: data de cadastro).
<br><br>

### 📋 *Módulo de Tarefas*
- Inserção, edição, exclusão e visualização de tarefas.
- Filtros:
  - Tarefas pendentes
  - Tarefas concluídas
  - Agrupamento por prioridade (Baixa, Normal, Alta)
- Atributos:
  - Título (2–100 caracteres)
  - Prioridade
  - Datas de criação e conclusão
  - Percentual de conclusão
  - Status
  - Itens de tarefa (opcionais)

#### ✅ *Itens de Tarefa*
- Adição e remoção de subtarefas.
- Conclusão de itens atualiza o progresso total da tarefa automaticamente.
<br><br>
![](https://i.redd.it/pcskrcaunm7f1.gif) 
***

## Tecnologias
![Tecnologias](https://skillicons.dev/icons?i=github,visualstudio,vscode,cs,dotnet,html,css,javascript,bootstrap)

***

## Como utilizar
1. Clone o repositório ou baixe o código fonte.

```
git clone https://github.com/Compila-logo-existe/eAgenda
```

2. Acesse a pasta do projeto:
   
```
cd PrototypeEAgenda.WebApp
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
dotnet run --project PrototypeEAgenda.WebApp
```

6. Execute o projeto:
   
```
[dotnet run --project PrototypeEAgenda.WebApp](https://localhost:5001)
```
## Requisitos

- .NET SDK 8.0 ou superior

- Navegador moderno (Edge, Chrome, Firefox etc.)

- Editor recomendado: Visual Studio 2022 ou superior (com suporte a ASP.NET MVC)

# Nunca mais tenha que correr contra o relógio, usando o Prototype E-Agenda
![](https://i.pinimg.com/originals/19/34/6d/19346dfd8992ad4fa57d7bd14a6f5210.gif) 
