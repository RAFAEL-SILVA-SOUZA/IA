USE [zeus]
GO
/****** Object:  Table [dbo].[Pergunta]    Script Date: 03/01/2021 21:51:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pergunta](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descricao] [varchar](8000) NULL,
 CONSTRAINT [PK_Pergunta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Resposta]    Script Date: 03/01/2021 21:51:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resposta](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descricao] [varchar](8000) NULL,
	[PerguntaId] [int] NULL,
 CONSTRAINT [PK_Resposta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
