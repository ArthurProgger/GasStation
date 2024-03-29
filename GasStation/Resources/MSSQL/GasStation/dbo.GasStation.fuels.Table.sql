CREATE TABLE [dbo].[fuels](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[fuel_name] [varchar](60) NOT NULL,
	[fuel_type] [nvarchar](50) NOT NULL,
	[fuel_manufacturer_id] [int] NOT NULL

	FOREIGN KEY([fuel_type]) REFERENCES [dbo].[fuel_types] ([t_name]) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY([fuel_manufacturer_id]) REFERENCES [dbo].[fuel_manufacturers] ([id]) ON UPDATE CASCADE ON DELETE CASCADE
)
