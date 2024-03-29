USE [GasStationShop]
GO
/****** Object:  Table [dbo].[units]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[units](
	[u_s_name] [varchar](5) NOT NULL,
	[u_name] [varchar](25) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[u_s_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
