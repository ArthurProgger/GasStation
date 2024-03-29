USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products_units]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_units](
	[prod_id] [int] NOT NULL,
	[unit] [varchar](5) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[products_units]  WITH CHECK ADD FOREIGN KEY([prod_id])
REFERENCES [dbo].[products] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[products_units]  WITH CHECK ADD FOREIGN KEY([unit])
REFERENCES [dbo].[units] ([u_s_name])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
