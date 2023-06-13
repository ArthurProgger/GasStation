create proc get_fuel_types_of_columns @col_id int = -1 as

	if (@col_id = -1)
		select
			gas_columns_fuel_types.gas_column_id as Номер,
			gas_columns_fuel_types.fuel_type as [Тип топлива]
		from
			gas_columns_fuel_types
	else
		select
			gas_columns_fuel_types.gas_column_id as Номер,
			gas_columns_fuel_types.fuel_type as [Тип топлива]
		from
			gas_columns_fuel_types
		where
			gas_columns_fuel_types.gas_column_id = @col_id
