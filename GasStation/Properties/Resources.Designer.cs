﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GasStation.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GasStation.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap add_product {
            get {
                object obj = ResourceManager.GetObject("add_product", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap add_supply {
            get {
                object obj = ResourceManager.GetObject("add_supply", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap add_type {
            get {
                object obj = ResourceManager.GetObject("add_type", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap back_arrow {
            get {
                object obj = ResourceManager.GetObject("back_arrow", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap count {
            get {
                object obj = ResourceManager.GetObject("count", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[columns_fuel_types](
        ///	[gas_column_id] [int] NOT NULL,
        ///	[fuel_type] [nvarchar](50) NOT NULL
        ///
        ///	foreign key ([gas_column_id]) REFERENCES [dbo].[gas_columns] ([id]) ON UPDATE CASCADE,
        ///	foreign key ([fuel_type]) REFERENCES [dbo].[fuel_types] ([t_name]) ON UPDATE CASCADE ON DELETE CASCADE
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_columns_fuel_types_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_columns_fuel_types_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[columns_types](
        ///	[col_type_name] [varchar](40) NOT NULL PRIMARY KEY
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_columns_types_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_columns_types_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_coming](
        ///	[date_time] [datetime] NOT NULL UNIQUE,
        ///	[t_name] [nvarchar](50) NOT NULL,
        ///	[fuel_count] [float] NOT NULL,
        ///	[fuel_id] [int] NOT NULL
        ///
        ///	FOREIGN KEY ([t_name]) REFERENCES [dbo].[fuel_types] ([t_name]),
        ///	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id])
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_coming_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_coming_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_manufacturers](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[man_name] [varchar](65) NULL
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_manufacturers_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_manufacturers_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_suppliers](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[supp_name] [varchar](65) NULL,
        ///	[inn] [bigint] NOT NULL,
        ///	[contract_file] [binary](1) NULL,
        ///	[contract_file_path] [nvarchar](max) NULL
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_suppliers_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_suppliers_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_supplies_content](
        ///	[fuel_supp_id] [int] NOT NULL,
        ///	[fuel_id] [int] NOT NULL,
        ///	[f_count] [float] NOT NULL
        ///
        ///	FOREIGN KEY([fuel_supp_id]) REFERENCES [dbo].[fuel_supplies] ([id]) ON UPDATE CASCADE ON DELETE CASCADE,
        ///	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id]) ON UPDATE CASCADE
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_supplies_content_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_supplies_content_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_supplies](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[date_time] [datetime] NOT NULL,
        ///	[supp_id] [int] NOT NULL
        ///
        ///	FOREIGN KEY([supp_id]) REFERENCES [dbo].[fuel_suppliers] ([id]) ON UPDATE CASCADE ON DELETE CASCADE
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_supplies_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_supplies_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_types](
        ///	[t_name] [nvarchar](50) PRIMARY KEY
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_types_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_types_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuel_using](
        ///	[date_time] [datetime] NOT NULL UNIQUE,
        ///	[t_name] [nvarchar](50) NOT NULL,
        ///	[fuel_count] [float] NOT NULL
        ///
        ///	FOREIGN KEY([t_name]) REFERENCES [dbo].[fuel_types] ([t_name])
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuel_using_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuel_using_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuels_prices](
        ///	[price_date] [date] NOT NULL UNIQUE,
        ///	[fuel_id] [int] NOT NULL,
        ///	[price] [money] NOT NULL
        ///
        ///	FOREIGN KEY([fuel_id]) REFERENCES [dbo].[fuels] ([id])
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuels_prices_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuels_prices_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[fuels](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[fuel_name] [varchar](60) NOT NULL,
        ///	[fuel_type] [nvarchar](50) NOT NULL,
        ///	[fuel_manufacturer_id] [int] NOT NULL
        ///
        ///	FOREIGN KEY([fuel_type]) REFERENCES [dbo].[fuel_types] ([t_name]) ON UPDATE CASCADE ON DELETE CASCADE,
        ///	FOREIGN KEY([fuel_manufacturer_id]) REFERENCES [dbo].[fuel_manufacturers] ([id]) ON UPDATE CASCADE ON DELETE CASCADE
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_fuels_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_fuels_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE gas_columns_fuel_types
        ///(
        ///	gas_column_id int not null,
        ///	fuel_type nvarchar(50) not null
        ///
        ///	foreign key (gas_column_id) references gas_columns(id),
        ///	foreign key (fuel_type) references fuel_types(t_name)
        ///).
        /// </summary>
        internal static string dbo_GasStation_gas_columns_fuel_types_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_gas_columns_fuel_types_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[gas_columns](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[col_type] [varchar](40) NOT NULL
        ///
        ///	FOREIGN KEY([col_type]) REFERENCES [dbo].[columns_types] ([col_type_name])
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_gas_columns_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_gas_columns_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE proc [dbo].[get_fuel_count] @fuel_type nvarchar(50) as
        ///
        ///	select
        ///		(select
        ///			SUM(fuel_coming.fuel_count)
        ///		from
        ///			fuel_coming
        ///		where
        ///			fuel_coming.t_name = @fuel_type)
        ///		-
        ///		(select
        ///			SUM(fuel_using.fuel_count)
        ///		from
        ///			fuel_using
        ///		where
        ///			fuel_using.t_name = @fuel_type).
        /// </summary>
        internal static string dbo_GasStation_get_fuel_count_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_fuel_count_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc get_fuel_types_of_columns @col_id int = -1 as
        ///
        ///	if (@col_id = -1)
        ///		select
        ///			gas_columns_fuel_types.gas_column_id as �����,
        ///			gas_columns_fuel_types.fuel_type as [��� �������]
        ///		from
        ///			gas_columns_fuel_types
        ///	else
        ///		select
        ///			gas_columns_fuel_types.gas_column_id as �����,
        ///			gas_columns_fuel_types.fuel_type as [��� �������]
        ///		from
        ///			gas_columns_fuel_types
        ///		where
        ///			gas_columns_fuel_types.gas_column_id = @col_id
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_fuel_types_of_columns_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_fuel_types_of_columns_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_fuels_prices] @fuel_id int as
        ///	
        ///	select
        ///		fuels_prices.price_date as Период,
        ///		fuels_prices.price as Цена
        ///	from
        ///		fuels_prices
        ///	where
        ///		fuels_prices.fuel_id = @fuel_id
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_fuels_prices_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_fuels_prices_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE proc [dbo].[get_fuels] @fuel_name varchar(60) = &apos;&apos;, @fuel_manufacturer_id int = -1, @fuel_type nvarchar(50) = null as
        ///
        ///	if (@fuel_manufacturer_id = -1)
        ///	begin
        ///		if (@fuel_type is null)
        ///			select
        ///				fuels.id as Код,
        ///				fuels.fuel_name as Название,
        ///				fuels.fuel_manufacturer_id as Производитель,
        ///				fuels.fuel_type as Тип,
        ///				(select top(1)
        ///					fuels_prices.price
        ///				from
        ///					fuels_prices
        ///				where
        ///					fuels_prices.fuel_id = fuels.id
        ///				order by
        ///					fuels_prices.price_date des [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string dbo_GasStation_get_fuels_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_fuels_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_fuels_types] as
        ///
        ///	select
        ///		fuel_types.t_name as Топливо
        ///	from 
        ///		fuel_types
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_fuels_types_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_fuels_types_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE proc [dbo].[get_gas_columns] @col_type varchar(40) as
        ///
        ///	select
        ///		gas_columns.id as Код,
        ///		gas_columns.col_type as Тип,
        ///		columns_fuel_types.fuel_type as [Тип топлива]
        ///	from 
        ///		gas_columns
        ///	left join columns_fuel_types on columns_fuel_types.gas_column_id = gas_columns.id
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_gas_columns_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_gas_columns_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_manufacturers] as
        ///
        ///	select
        ///		fuel_manufacturers.id as Код,
        ///		fuel_manufacturers.man_name as Название
        ///	from
        ///		fuel_manufacturers
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_manufacturers_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_manufacturers_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_posts] as
        ///
        ///	select
        ///		posts.p_name as Название,
        ///		posts.descr as Описание
        ///	from
        ///		posts.
        /// </summary>
        internal static string dbo_GasStation_get_posts_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_posts_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_sex] as
        ///
        ///	select
        ///		sex.s_name as Пол
        ///	from
        ///		sex
        ///.
        /// </summary>
        internal static string dbo_GasStation_get_sex_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_sex_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_stuffers] @l_name varchar(50) = &apos;&apos; , @f_name varchar(50) = &apos;&apos; , @m_name varchar(50) = &apos;&apos; , @sex varchar = null, @post varchar(50) = null as
        ///
        ///	if (@sex is null)
        ///	begin
        ///		if (@post is null)
        ///			select
        ///				stuffers.id As Код,
        ///				stuffers.l_name as Фамилия,
        ///				stuffers.f_name as Имя,
        ///				stuffers.m_name as Отчество,
        ///				stuffers.bitrh as [Дата рождения],
        ///				stuffers.p_series as [Серия паспорта],
        ///				stuffers.p_num as [Номер паспорта],
        ///				stuffers.p_gived as [Орган, выдавши [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string dbo_GasStation_get_stuffers_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_stuffers_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc [dbo].[get_suppliers] @supp_name varchar(65) = &apos;&apos; as
        ///
        ///	select
        ///		fuel_suppliers.id as Код,
        ///		fuel_suppliers.supp_name as Название,
        ///		fuel_suppliers.inn as ИНН,
        ///		fuel_suppliers.contract_file as Договор,
        ///		fuel_suppliers.contract_file_path as Договор,
        ///		fuel_supplies.date_time as [Дата заказа],
        ///		fuels.fuel_name as Топливо,
        ///		fuel_supplies_content.f_count as Количество
        ///	from
        ///		fuel_suppliers
        ///	left join fuel_supplies on fuel_supplies.supp_id = fuel_suppliers.id
        ///	left join fuel_supplie [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string dbo_GasStation_get_suppliers_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_get_suppliers_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[posts](
        ///	[p_name] [varchar](50) NOT NULL PRIMARY KEY,
        ///	[descr] [varchar](max) NULL
        ///).
        /// </summary>
        internal static string dbo_GasStation_posts_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_posts_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[sex](
        ///	[s_s_name] [varchar](1) NOT NULL PRIMARY KEY,
        ///	[s_name] [varchar](7) NOT NULL
        ///)
        ///.
        /// </summary>
        internal static string dbo_GasStation_sex_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_sex_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на CREATE TABLE [dbo].[stuffers](
        ///	[id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ///	[l_name] [varchar](50) NOT NULL,
        ///	[f_name] [varchar](50) NOT NULL,
        ///	[m_name] [varchar](50) NOT NULL,
        ///	[bitrh] [date] NOT NULL,
        ///	[p_series] [int] NOT NULL,
        ///	[p_num] [int] NOT NULL,
        ///	[p_gived] [varchar](120) NOT NULL,
        ///	[sex] [varchar](1) NOT NULL,
        ///	[contract_file] [binary](1) NULL,
        ///	[contract_file_path] [nvarchar](max) NULL,
        ///	[post] [varchar](50) NOT NULL
        ///
        ///	FOREIGN KEY([post]) REFERENCES [dbo].[posts] ([p_name]),
        ///	F [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string dbo_GasStation_stuffers_Table {
            get {
                return ResourceManager.GetString("dbo_GasStation_stuffers_Table", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc get_tanks_volume @f_type nvarchar(50) = null as
        ///	if (@f_type is null)
        ///		select distinct
        ///			fuel_types.t_name as [Тип топлива],
        ///			fuel_types.max_volume as [Максимальный объем],
        ///			((select
        ///				SUM(fuel_coming.fuel_count)
        ///			from
        ///				fuel_coming
        ///			where
        ///				fuel_coming.t_name = fuels.fuel_type)
        ///			-
        ///			(select
        ///				SUM(fuel_using.fuel_count)
        ///			from
        ///				fuel_using
        ///			where
        ///				fuel_using.t_name = fuels.fuel_type)) as Объем
        ///		from
        ///			fuel_types
        ///		left join fuels on fuel_types.t [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string dbo_GasStation_tanks_volume_StoredProcedure {
            get {
                return ResourceManager.GetString("dbo_GasStation_tanks_volume_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap download {
            get {
                object obj = ResourceManager.GetObject("download", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap edit {
            get {
                object obj = ResourceManager.GetObject("edit", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap gas_station_icon {
            get {
                object obj = ResourceManager.GetObject("gas_station_icon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc get_columns_table(@t_name nvarchar(100)) as
        ///
        ///	select
        ///		COLUMN_NAME,
        ///		DATA_TYPE,
        ///		CHARACTER_MAXIMUM_LENGTH
        ///	from
        ///		INFORMATION_SCHEMA.COLUMNS
        ///	where
        ///		TABLE_NAME = @t_name.
        /// </summary>
        internal static string get_columns_table_StoredProcedure {
            get {
                return ResourceManager.GetString("get_columns_table_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc get_foreign_columns @t_name nvarchar(100) as
        ///
        ///	select
        ///		INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME,
        ///		INFORMATION_SCHEMA.KEY_COLUMN_USAGE.TABLE_NAME,
        ///		INFORMATION_SCHEMA.KEY_COLUMN_USAGE.COLUMN_NAME
        ///	from
        ///		sys.tables
        ///	left join sys.foreign_keys on sys.foreign_keys.parent_object_id = sys.tables.[object_id]
        ///	left join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS on INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.CONSTRAINT_NAME = sys.foreign_keys.[name]
        ///	left join INFORMATION_SCHE [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string get_foreign_columns_StoredProcedure {
            get {
                return ResourceManager.GetString("get_foreign_columns_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на create proc get_foreign_tables(@p_t_name nvarchar(100)) as
        ///
        ///	select
        ///		sys.tables.[name]
        ///	from
        ///		INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
        ///	left join INFORMATION_SCHEMA.TABLE_CONSTRAINTS on INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.UNIQUE_CONSTRAINT_NAME = INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME
        ///	left join sys.foreign_keys on sys.foreign_keys.[name] = INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS.CONSTRAINT_NAME
        ///	left join sys.tables on sys.tables.[object_id] = sys.foreign_keys.parent_obj [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string get_foreign_tables_StoredProcedure {
            get {
                return ResourceManager.GetString("get_foreign_tables_StoredProcedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap info {
            get {
                object obj = ResourceManager.GetObject("info", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap no_photos {
            get {
                object obj = ResourceManager.GetObject("no_photos", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap product_range {
            get {
                object obj = ResourceManager.GetObject("product_range", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap sale {
            get {
                object obj = ResourceManager.GetObject("sale", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap save_pdf {
            get {
                object obj = ResourceManager.GetObject("save_pdf", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap shopping_basket {
            get {
                object obj = ResourceManager.GetObject("shopping_basket", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap supp_add {
            get {
                object obj = ResourceManager.GetObject("supp_add", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap team {
            get {
                object obj = ResourceManager.GetObject("team", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap truck {
            get {
                object obj = ResourceManager.GetObject("truck", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap write {
            get {
                object obj = ResourceManager.GetObject("write", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Byte[].
        /// </summary>
        internal static byte[] Шаблон_договора_с_поставщиками {
            get {
                object obj = ResourceManager.GetObject("Шаблон_договора_с_поставщиками", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Byte[].
        /// </summary>
        internal static byte[] Шаблон_трудового_договора {
            get {
                object obj = ResourceManager.GetObject("Шаблон_трудового_договора", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
