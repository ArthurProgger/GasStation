USE [GasStationShop]
GO
/****** Object:  Table [dbo].[products_supplies]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_supplies](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date_time] [datetime] NOT NULL,
	[supp_id] [int] NOT NULL,
	[buyer_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[products_supplies]  WITH CHECK ADD FOREIGN KEY([supp_id])
REFERENCES [dbo].[products_suppliers] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
