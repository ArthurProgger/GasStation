CREATE TABLE [dbo].[stuffers](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[l_name] [varchar](50) NOT NULL,
	[f_name] [varchar](50) NOT NULL,
	[m_name] [varchar](50) NOT NULL,
	[bitrh] [date] NOT NULL,
	[p_series] [int] NOT NULL,
	[p_num] [int] NOT NULL,
	[p_gived] [varchar](120) NOT NULL,
	[sex] [varchar](1) NOT NULL,
	[contract_file] [binary](1) NULL,
	[contract_file_path] [nvarchar](max) NULL,
	[post] [varchar](50) NOT NULL

	FOREIGN KEY([post]) REFERENCES [dbo].[posts] ([p_name]),
	FOREIGN KEY([sex]) REFERENCES [dbo].[sex] ([s_s_name])
)
