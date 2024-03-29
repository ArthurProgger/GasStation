USE [GasStationShop]
GO
/****** Object:  Table [dbo].[units_expressions]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[units_expressions](
	[unit1] [varchar](5) NOT NULL,
	[val1] [float] NOT NULL,
	[unit2] [varchar](5) NOT NULL,
	[val2] [float] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[units_expressions]  WITH CHECK ADD FOREIGN KEY([unit2])
REFERENCES [dbo].[units] ([u_s_name])
GO
ALTER TABLE [dbo].[units_expressions]  WITH CHECK ADD FOREIGN KEY([unit1])
REFERENCES [dbo].[units] ([u_s_name])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
