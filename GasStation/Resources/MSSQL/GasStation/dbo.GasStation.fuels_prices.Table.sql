CREATE TABLE [dbo].[fuels_prices](
	[price_date] [date] NOT NULL UNIQUE,
	[fuel_id] [int] NOT NULL,
	[price] [money] NOT NULL

	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id])
)
