CREATE TABLE [dbo].[fuel_coming](
	[date_time] [datetime] NOT NULL UNIQUE,
	[t_name] [nvarchar](50) NOT NULL,
	[fuel_count] [float] NOT NULL,
	[fuel_id] [int] NOT NULL

	FOREIGN KEY ([t_name]) REFERENCES [dbo].[fuel_types] ([t_name]),
	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id])
)
