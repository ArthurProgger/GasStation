create proc [dbo].[get_manufacturers] as

	select
		fuel_manufacturers.id as Код,
		fuel_manufacturers.man_name as Название
	from
		fuel_manufacturers
