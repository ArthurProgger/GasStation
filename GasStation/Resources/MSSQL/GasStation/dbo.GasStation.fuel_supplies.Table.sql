CREATE TABLE [dbo].[fuel_supplies](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[date_time] [datetime] NOT NULL,
	[supp_id] [int] NOT NULL

	FOREIGN KEY([supp_id]) REFERENCES [dbo].[fuel_suppliers] ([id]) ON UPDATE CASCADE ON DELETE CASCADE
)
