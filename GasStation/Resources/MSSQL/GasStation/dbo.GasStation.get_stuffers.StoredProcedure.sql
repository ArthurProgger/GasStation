create proc [dbo].[get_stuffers] @l_name varchar(50) = '' , @f_name varchar(50) = '' , @m_name varchar(50) = '' , @sex varchar = null, @post varchar(50) = null as

	if (@sex is null)
	begin
		if (@post is null)
			select
				stuffers.id As Код,
				stuffers.l_name as Фамилия,
				stuffers.f_name as Имя,
				stuffers.m_name as Отчество,
				stuffers.bitrh as [Дата рождения],
				stuffers.p_series as [Серия паспорта],
				stuffers.p_num as [Номер паспорта],
				stuffers.p_gived as [Орган, выдавший паспорт],
				stuffers.sex as Пол,
				stuffers.contract_file as Договор,
				stuffers.contract_file_path as Договор,
				stuffers.post as Должность
			from
				stuffers
			where
				stuffers.l_name like @l_name + '%' and
				stuffers.f_name like @f_name + '%' and
				stuffers.m_name like @m_name + '%'
		else
						select
				stuffers.id As Код,
				stuffers.l_name as Фамилия,
				stuffers.f_name as Имя,
				stuffers.m_name as Отчество,
				stuffers.bitrh as [Дата рождения],
				stuffers.p_series as [Серия паспорта],
				stuffers.p_num as [Номер паспорта],
				stuffers.p_gived as [Орган, выдавший паспорт],
				stuffers.sex as Пол,
				stuffers.contract_file as Договор,
				stuffers.contract_file_path as Договор,
				stuffers.post as Должность
			from
				stuffers
			where
				stuffers.l_name like @l_name + '%' and
				stuffers.f_name like @f_name + '%' and
				stuffers.m_name like @m_name + '%' and
				stuffers.post = @post
	end
	else
	begin
		if (@post is null)
			select
				stuffers.id As Код,
				stuffers.l_name as Фамилия,
				stuffers.f_name as Имя,
				stuffers.m_name as Отчество,
				stuffers.bitrh as [Дата рождения],
				stuffers.p_series as [Серия паспорта],
				stuffers.p_num as [Номер паспорта],
				stuffers.p_gived as [Орган, выдавший паспорт],
				stuffers.sex as Пол,
				stuffers.contract_file as Договор,
				stuffers.contract_file_path as Договор,
				stuffers.post as Должность
			from
				stuffers
			where
				stuffers.l_name like @l_name + '%' and
				stuffers.f_name like @f_name + '%' and
				stuffers.m_name like @m_name + '%' and
				stuffers.sex = @sex
		else
						select
				stuffers.id As Код,
				stuffers.l_name as Фамилия,
				stuffers.f_name as Имя,
				stuffers.m_name as Отчество,
				stuffers.bitrh as [Дата рождения],
				stuffers.p_series as [Серия паспорта],
				stuffers.p_num as [Номер паспорта],
				stuffers.p_gived as [Орган, выдавший паспорт],
				stuffers.sex as Пол,
				stuffers.contract_file as Договор,
				stuffers.contract_file_path as Договор,
				stuffers.post as Должность
			from
				stuffers
			where
				stuffers.l_name like @l_name + '%' and
				stuffers.f_name like @f_name + '%' and
				stuffers.m_name like @m_name + '%' and
				stuffers.post = @post and
				stuffers.sex = @sex
	end