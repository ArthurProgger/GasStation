CREATE TABLE [dbo].[fuel_supplies_content](
	[fuel_supp_id] [int] NOT NULL,
	[fuel_id] [int] NOT NULL,
	[f_count] [float] NOT NULL

	FOREIGN KEY([fuel_supp_id]) REFERENCES [dbo].[fuel_supplies] ([id]) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id]) ON UPDATE CASCADE
)
