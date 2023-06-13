create proc get_columns_table(@t_name nvarchar(100)) as

	select
		COLUMN_NAME,
		DATA_TYPE,
		CHARACTER_MAXIMUM_LENGTH
	from
		INFORMATION_SCHEMA.COLUMNS
	where
		TABLE_NAME = @t_name