CREATE proc [dbo].[get_fuels] @fuel_name varchar(60) = '', @fuel_manufacturer_id int = -1, @fuel_type nvarchar(50) = null as

	if (@fuel_manufacturer_id = -1)
	begin
		if (@fuel_type is null)
			select
				fuels.id as Код,
				fuels.fuel_name as Название,
				fuels.fuel_manufacturer_id as Производитель,
				fuels.fuel_type as Тип,
				(select top(1)
					fuels_prices.price
				from
					fuels_prices
				where
					fuels_prices.fuel_id = fuels.id
				order by
					fuels_prices.price_date desc) as Цена,
				(select
					fuel_supplies_content.f_count
				from
					fuel_supplies_content
				where
					fuel_supplies_content.fuel_id = fuels.id)
				-
				(select
					fuel_coming.fuel_count
				from
					fuel_coming
				where
					fuel_coming.fuel_id = fuels.id) as Остаток
			from
				fuels
			where
				fuels.fuel_name like @fuel_name + '%'
		else
			select
				fuels.id as Код,
				fuels.fuel_name as Название,
				fuels.fuel_manufacturer_id as Производитель,
				fuels.fuel_type as Тип,
				(select top(1)
					fuels_prices.price
				from
					fuels_prices
				where
					fuels_prices.fuel_id = fuels.id
				order by
					fuels_prices.price_date desc) as Цена,
				(select
					fuel_supplies_content.f_count
				from
					fuel_supplies_content
				where
					fuel_supplies_content.fuel_id = fuels.id)
				-
				(select
					fuel_coming.fuel_count
				from
					fuel_coming
				where
					fuel_coming.fuel_id = fuels.id) as Остаток
			from
				fuels
			where
				fuels.fuel_type = @fuel_type and fuels.fuel_name like @fuel_name + '%'
	end
	else
	begin
		if (@fuel_type is null)
			select
				fuels.id as Код,
				fuels.fuel_name as Название,
				fuels.fuel_manufacturer_id as Производитель,
				fuels.fuel_type as Тип,
				(select top(1)
					fuels_prices.price
				from
					fuels_prices
				where
					fuels_prices.fuel_id = fuels.id
				order by
					fuels_prices.price_date desc) as Цена,
				(select
					fuel_supplies_content.f_count
				from
					fuel_supplies_content
				where
					fuel_supplies_content.fuel_id = fuels.id)
				-
				(select
					fuel_coming.fuel_count
				from
					fuel_coming
				where
					fuel_coming.fuel_id = fuels.id) as Остаток
			from
				fuels
			where
				fuels.fuel_name like @fuel_name + '%'
		else
			select
				fuels.id as Код,
				fuels.fuel_name as Название,
				fuels.fuel_manufacturer_id as Производитель,
				fuels.fuel_type as Тип,
				(select top(1)
					fuels_prices.price
				from
					fuels_prices
				where
					fuels_prices.fuel_id = fuels.id
				order by
					fuels_prices.price_date desc) as Цена,
				(select
					fuel_supplies_content.f_count
				from
					fuel_supplies_content
				where
					fuel_supplies_content.fuel_id = fuels.id)
				-
				(select
					fuel_coming.fuel_count
				from
					fuel_coming
				where
					fuel_coming.fuel_id = fuels.id) as Остаток
			from
				fuels
			where
				fuels.fuel_type = @fuel_type and fuels.fuel_manufacturer_id = @fuel_manufacturer_id and fuels.fuel_name like @fuel_name + '%'
	end
