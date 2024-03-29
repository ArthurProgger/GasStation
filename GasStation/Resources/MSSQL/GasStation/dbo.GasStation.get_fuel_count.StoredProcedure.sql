CREATE proc [dbo].[get_fuel_count] @fuel_type nvarchar(50) as

	select
		(select
			SUM(fuel_coming.fuel_count)
		from
			fuel_coming
		where
			fuel_coming.t_name = @fuel_type)
		-
		(select
			SUM(fuel_using.fuel_count)
		from
			fuel_using
		where
			fuel_using.t_name = @fuel_type)