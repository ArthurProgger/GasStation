CREATE TABLE [dbo].[gas_columns](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[col_type] [varchar](40) NOT NULL

	FOREIGN KEY([col_type]) REFERENCES [dbo].[columns_types] ([col_type_name])
)
