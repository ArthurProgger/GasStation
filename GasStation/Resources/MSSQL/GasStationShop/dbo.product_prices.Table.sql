USE [GasStationShop]
GO
/****** Object:  Table [dbo].[product_prices]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_prices](
	[price_date] [date] NOT NULL,
	[prod_id] [int] NOT NULL,
	[price] [money] NOT NULL,
UNIQUE NONCLUSTERED 
(
	[price_date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[product_prices]  WITH CHECK ADD FOREIGN KEY([prod_id])
REFERENCES [dbo].[products] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
