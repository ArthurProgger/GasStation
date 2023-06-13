CREATE TABLE [dbo].[fuel_suppliers](
	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[supp_name] [varchar](65) NULL,
	[inn] [bigint] NOT NULL,
	[contract_file] [binary](1) NULL,
	[contract_file_path] [nvarchar](max) NULL
)
