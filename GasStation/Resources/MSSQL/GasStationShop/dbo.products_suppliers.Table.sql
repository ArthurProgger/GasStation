USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products_suppliers]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_suppliers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[supp_name] [varchar](65) NOT NULL,
	[inn] [bigint] NOT NULL,
	[contract_file] [binary](1) NULL,
	[contract_file_path] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
