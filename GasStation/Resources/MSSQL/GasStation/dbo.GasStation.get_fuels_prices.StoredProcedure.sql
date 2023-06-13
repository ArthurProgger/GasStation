create proc [dbo].[get_fuels_prices] @fuel_id int as
	
	select
		fuels_prices.price_date as Период,
		fuels_prices.price as Цена
	from
		fuels_prices
	where
		fuels_prices.fuel_id = @fuel_id
