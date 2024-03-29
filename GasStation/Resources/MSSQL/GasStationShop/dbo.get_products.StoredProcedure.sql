USE [GasStationShop]
GO
/****** Object:  StoredProcedure [dbo].[get_products]    Script Date: 20.04.2023 11:40:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[get_products] @p_name varchar(75) = '' , @p_type varchar(60) = null as

	if (@p_type is null)
		select
			products.id as Код,
			products.p_name as Название,
			products.descr as Описание,
			products.prod_type as Тип,
			((select
				SUM(products_supplies_content.prod_count)
			from
				products_supplies_content
			where
				products_supplies_content.prod_id = products.id and products_supplies_content.unit = products_units.unit)
			-
			(select
				SUM(sales_content.prod_count)
			from
				sales_content
			where
				sales_content.prod_id = products.id and sales_content.unit = products_units.unit)) as Количество,
			products_units.unit as [Единица измерения],
			(select top(1)
				product_prices.price
			where
				product_prices.prod_id = products.id
			order by
				product_prices.price desc) as [Цена руб.]
		from
			products
		left join products_units on products_units.prod_id = products.id
		left join product_prices on product_prices.prod_id = products.id
		where
			products.p_name like @p_name + '%'
	else
		select
			products.id as Код,
			products.p_name as Название,
			products.descr as Описание,
			products.prod_type as Тип,
			((select
				SUM(products_supplies_content.prod_count)
			from
				products_supplies_content
			where
				products_supplies_content.prod_id = products.id and products_supplies_content.unit = products_units.unit)
			-
			(select
				SUM(sales_content.prod_count)
			from
				sales_content
			where
				sales_content.prod_id = products.id and sales_content.unit = products_units.unit)) as Количество,
			products_units.unit as [Единица измерения],
			(select top(1)
				product_prices.price
			where
				product_prices.prod_id = products.id
			order by
				product_prices.price desc) as [Цена руб.]
		from
			products
		left join products_units on products_units.prod_id = products.id
		left join product_prices on product_prices.prod_id = products.id
		where
			products.p_name like @p_name + '%' and products.prod_type = @p_type
GO
