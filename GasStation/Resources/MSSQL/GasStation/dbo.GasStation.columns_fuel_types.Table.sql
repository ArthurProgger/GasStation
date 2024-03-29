CREATE TABLE [dbo].[columns_fuel_types](
	[gas_column_id] [int] NOT NULL,
	[fuel_type] [nvarchar](50) NOT NULL

	foreign key ([gas_column_id]) REFERENCES [dbo].[gas_columns] ([id]) ON UPDATE CASCADE,
	foreign key ([fuel_type]) REFERENCES [dbo].[fuel_types] ([t_name]) ON UPDATE CASCADE ON DELETE CASCADE
)
