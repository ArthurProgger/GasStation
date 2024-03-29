USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[p_name] [varchar](75) NOT NULL,
	[descr] [varchar](max) NULL,
	[prod_type] [varchar](60) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([prod_type])
REFERENCES [dbo].[products_types] ([t_name])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
