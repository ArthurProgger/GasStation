USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products_types]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_types](
	[t_name] [varchar](60) NOT NULL,
	[descr] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[t_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
