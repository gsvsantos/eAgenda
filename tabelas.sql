CREATE TABLE [dbo].[TBContato] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Nome]     NVARCHAR (100)   NOT NULL,
    [Telefone] NVARCHAR (20)    NOT NULL,
    [Email]    NVARCHAR (50)    NOT NULL,
    [Empresa]  NVARCHAR (100)   NULL,
    [Cargo]    NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBCompromisso] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Assunto]     NVARCHAR (100)   NOT NULL,
    [Data]        DATETIME2 (7)    NOT NULL,
    [HoraInicio]  BIGINT           NOT NULL,
    [HoraTermino] BIGINT           NOT NULL,
    [Tipo]        INT              NOT NULL,
    [Local]       NVARCHAR (100)   NULL,
    [Link]        NVARCHAR (200)   NULL,
    [Contato_Id]  UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TBCompromisso_TBContato] FOREIGN KEY ([Contato_Id]) REFERENCES [dbo].[TBContato] ([Id])
);

CREATE TABLE [dbo].[TBDespesa] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Titulo]         NVARCHAR (100)   NOT NULL,
    [Descricao]      NVARCHAR (100)   NOT NULL,
    [DataOcorrencia] DATETIME2 (7)    NOT NULL,
    [Valor]          DECIMAL (18, 2)  NOT NULL,
    [FormaPagamento] INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBCategoria] (
    [Id]     UNIQUEIDENTIFIER NOT NULL,
    [Titulo] NVARCHAR (100)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBDespesa_TBCategoria] (
    [Despesa_Id]   UNIQUEIDENTIFIER NOT NULL,
    [Categoria_Id] UNIQUEIDENTIFIER NOT NULL
);

CREATE TABLE [dbo].[TBTarefa] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Titulo]        NVARCHAR (100)   NOT NULL,
    [Descricao]     NVARCHAR (100)   NOT NULL,
    [Prioridade]    INT              NOT NULL,
    [DataCriacao]   DATETIME2 (7)    NOT NULL,
    [DataConclusao] DATETIME2 (7)    NULL,
    [Status]        INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBItemTarefa] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Titulo]    NVARCHAR (100)   NOT NULL,
    [Status]    INT              NOT NULL,
    [Tarefa_Id] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TBItemTarefa_TBTarefa] FOREIGN KEY ([Tarefa_Id]) REFERENCES [dbo].[TBTarefa] ([Id])
);

