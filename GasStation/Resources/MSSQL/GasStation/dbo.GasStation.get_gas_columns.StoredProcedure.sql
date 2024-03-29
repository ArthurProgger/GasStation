CREATE proc [dbo].[get_gas_columns] @col_type varchar(40) as

	select
		gas_columns.id as Код,
		gas_columns.col_type as Тип,
		columns_fuel_types.fuel_type as [Тип топлива]
	from 
		gas_columns
	left join columns_fuel_types on columns_fuel_types.gas_column_id = gas_columns.id
