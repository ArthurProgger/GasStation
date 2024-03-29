create proc [dbo].[get_suppliers] @supp_name varchar(65) = '' as

	select
		fuel_suppliers.id as Код,
		fuel_suppliers.supp_name as Название,
		fuel_suppliers.inn as ИНН,
		fuel_suppliers.contract_file as Договор,
		fuel_suppliers.contract_file_path as Договор,
		fuel_supplies.date_time as [Дата заказа],
		fuels.fuel_name as Топливо,
		fuel_supplies_content.f_count as Количество
	from
		fuel_suppliers
	left join fuel_supplies on fuel_supplies.supp_id = fuel_suppliers.id
	left join fuel_supplies_content on fuel_supplies_content.fuel_supp_id = fuel_supplies.supp_id
	left join fuels on fuels.id = fuel_supplies_content.fuel_id
	where
		fuel_suppliers.supp_name like @supp_name + '%'
