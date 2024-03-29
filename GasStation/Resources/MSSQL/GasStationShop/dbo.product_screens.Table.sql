USE [GasStationShop]
GO
/****** Object:  Table [dbo].[product_screens]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product_screens](
	[prod_id] [int] NOT NULL,
	[screen_file] [binary](1) NULL,
	[screen_file_path] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[product_screens]  WITH CHECK ADD FOREIGN KEY([prod_id])
REFERENCES [dbo].[products] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
