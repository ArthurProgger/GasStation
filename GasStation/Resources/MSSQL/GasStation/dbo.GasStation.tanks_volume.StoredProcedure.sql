create proc get_tanks_volume @f_type nvarchar(50) = null as
	if (@f_type is null)
		select distinct
			fuel_types.t_name as [Тип топлива],
			fuel_types.max_volume as [Максимальный объем],
			((select
				SUM(fuel_coming.fuel_count)
			from
				fuel_coming
			where
				fuel_coming.t_name = fuels.fuel_type)
			-
			(select
				SUM(fuel_using.fuel_count)
			from
				fuel_using
			where
				fuel_using.t_name = fuels.fuel_type)) as Объем
		from
			fuel_types
		left join fuels on fuel_types.t_name = fuels.fuel_type
	else
		select distinct
			fuel_types.t_name as [Тип топлива],
			fuel_types.max_volume as [Максимальный объем],
			((select
				SUM(fuel_coming.fuel_count)
			from
				fuel_coming
			where
				fuel_coming.t_name = fuels.fuel_type)
			-
			(select
				SUM(fuel_using.fuel_count)
			from
				fuel_using
			where
				fuel_using.t_name = fuels.fuel_type)) as Объем
		from
			fuel_types
		left join fuels on fuel_types.t_name = fuels.fuel_type
		where
			fuel_types.t_name = @f_type