CREATE TABLE gas_columns_fuel_types
(
	gas_column_id int not null,
	fuel_type nvarchar(50) not null

	foreign key (gas_column_id) references gas_columns(id),
	foreign key (fuel_type) references fuel_types(t_name)
)