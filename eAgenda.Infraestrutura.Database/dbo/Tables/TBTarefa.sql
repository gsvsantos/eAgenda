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

