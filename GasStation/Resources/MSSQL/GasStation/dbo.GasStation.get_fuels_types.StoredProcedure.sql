create proc [dbo].[get_fuels_types] as

	select
		fuel_types.t_name as Топливо
	from 
		fuel_types
