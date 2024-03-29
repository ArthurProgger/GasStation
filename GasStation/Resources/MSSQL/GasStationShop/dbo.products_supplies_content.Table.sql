USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products_supplies_content]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_supplies_content](
	[supp_id] [int] NOT NULL,
	[prod_id] [int] NOT NULL,
	[prod_count] [float] NOT NULL,
	[unit] [varchar](5) NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[products_supplies_content]  WITH CHECK ADD FOREIGN KEY([prod_id])
REFERENCES [dbo].[products] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[products_supplies_content]  WITH CHECK ADD FOREIGN KEY([unit])
REFERENCES [dbo].[units] ([u_s_name])
GO
ALTER TABLE [dbo].[products_supplies_content]  WITH CHECK ADD  CONSTRAINT [FK_products_supplies_content_products_supplies] FOREIGN KEY([supp_id])
REFERENCES [dbo].[products_supplies] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[products_supplies_content] CHECK CONSTRAINT [FK_products_supplies_content_products_supplies]
GO
