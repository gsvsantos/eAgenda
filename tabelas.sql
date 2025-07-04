CREATE TABLE [dbo].[TBCompromisso] (
    [Id]          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
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

CREATE TABLE [dbo].[TBContato] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Nome]     NVARCHAR (100)   NOT NULL,
    [Telefone] NVARCHAR (20)    NOT NULL,
    [Email]    NVARCHAR (50)    NOT NULL,
    [Empresa]  NVARCHAR (100)   NULL,
    [Cargo]    NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBTarefa] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Titulo]        NVARCHAR(100) NOT NULL,
    [DataCriacao]   DATETIME2 NOT NULL,
    [DataConclusao] DATETIME2 NULL,
    [Concluida]     BIT NOT NULL
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[TBItemTarefa] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Titulo]   NVARCHAR(100) NOT NULL,
    [Concluido] BIT NOT NULL,
    [Tarefa_Id] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TBItemTarefa_TBTarefa] FOREIGN KEY ([Tarefa_Id]) REFERENCES [dbo].[TBTarefa] ([Id])
);