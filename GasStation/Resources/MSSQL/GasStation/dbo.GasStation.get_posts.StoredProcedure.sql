create proc [dbo].[get_posts] as

	select
		posts.p_name as Название,
		posts.descr as Описание
	from
		posts