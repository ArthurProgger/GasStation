create proc [dbo].[get_sex] as

	select
		sex.s_name as Пол
	from
		sex
