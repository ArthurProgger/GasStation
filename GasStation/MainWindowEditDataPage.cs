using System;
using System.Reflection;
using System.IO;
using System.Windows;
using SWF = System.Windows.Forms;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using XamlAnimatedGif;

namespace GasStation
{
    public partial class MainWindow
    {
        private void EditDataPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (editDataPage.Visibility == Visibility.Visible)
            {
                DataTable columns = new DataTable(), foreignColumns = new DataTable(), foreignTables = new DataTable(), data = new DataTable();
                try
                {
                    switch (App.SystemConfigs.SelectedDBMS)
                    {
                        case DBMS.MSSQL:
                            {
                                columns = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_columns_table '{_tableName}', {Convert.ToInt32(!_isInsert)}", App.SystemConfigs.ConnectionStr));
                                foreignColumns = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_foreign_columns '{_tableName}'", App.SystemConfigs.ConnectionStr));
                                foreignTables = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_foreign_tables '{_tableName}'", App.SystemConfigs.ConnectionStr));

                                break;
                            }
                        case DBMS.MySQL:
                            {
                                columns = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter($"call get_columns_table ('{_tableName}')", App.SystemConfigs.ConnectionStr));
                                foreignColumns = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter($"call get_foreign_columns ('{_tableName}')", App.SystemConfigs.ConnectionStr));

                                break;
                            }
                    }

                    int i;
                    for (i = 0; i < columns.Rows.Count; i++)
                        columns.Rows.Cast<DataRow>().Where(r => !Equals(r, columns.Rows[i]) && string.Equals(r[0].ToString(), columns.Rows[i][0].ToString())).ToList().ForEach((DataRow r) => columns.Rows.Remove(r));

                    if (!_isInsert)
                    {
                        _where = null;
                        i = 0;
                        columns.Rows.Cast<DataRow>().Where(r => !string.IsNullOrEmpty(r[3].ToString())).ToList().ForEach((DataRow r) =>
                        {
                            if (_keysValues[i] != null)
                                _where += string.IsNullOrEmpty(r[2].ToString()) ? $"{r[0]} = {_keysValues[i]} and " : $"{r[0]} = '{_keysValues[i]}' and ";
                            i++;
                        });
                        _where = _where.Substring(0, _where.Length - 5);
                        data = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"select * from {_tableName} where {_where}", App.SystemConfigs.ConnectionStr));
                    }

                    _columnsTypes.Add(_tableName, new Columns());

                    if (foreignColumns.Rows.Cast<DataRow>().Where(r => string.IsNullOrEmpty(r[0].ToString())).Count() > 0)
                        foreignColumns.Rows.Clear();
                    else
                        foreignColumns.PrimaryKey = new DataColumn[1] { foreignColumns.Columns[0] };

                    i = 0;
                    columns.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                    {
                        Label valueTitle = new Label
                        {
                            FontFamily = new FontFamily("pack://application:,,,/Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14
                        };

                        if (!string.IsNullOrEmpty(r[2].ToString()))
                            _columnsTypes[_tableName].StrColumns.Add(r[0].ToString());
                        else if (string.Equals(r[1].ToString(), "date"))
                            _columnsTypes[_tableName].DateColumns.Add(r[0].ToString());
                        else if (string.Equals(r[1].ToString(), "time"))
                            _columnsTypes[_tableName].TimeColumns.Add(r[0].ToString());
                        else if (string.Equals(r[1].ToString(), "datetime"))
                            _columnsTypes[_tableName].DateTimeColumns.Add(r[0].ToString());
                        else if (r[1].ToString().Contains("binary") || string.Equals(r[1].ToString(), "image"))
                            _columnsTypes[_tableName].BinaryColumns.Add(r[0].ToString());

                        if (_tableSynonyms.ContainsKey(_tableName))
                            valueTitle.Content = _tableSynonyms[_tableName].ColumnsSynonyms.ContainsKey(r[0].ToString()) ? _tableSynonyms[_tableName].ColumnsSynonyms[r[0].ToString()] : r[0];
                        else
                            valueTitle.Content = r[0];

                        Grid.SetColumn(valueTitle, 0);
                        editDataPageContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                        Grid.SetRow(valueTitle, editDataPageContent.RowDefinitions.Count - 1);

                        editDataPageContent.Children.Add(valueTitle);

                        var rows = from row in foreignColumns.AsEnumerable()
                                   where string.Equals(row[0].ToString(), r[0].ToString())
                                   select row;

                        if (rows.Count() > 0)
                            CreateValueContainer<ComboBox>(r[0].ToString(), rows.ToArray()[0][1].ToString(), rows.ToArray()[0][2].ToString());
                        else
                        {
                            if (_columnsTypes[_tableName].DateTimeColumns.Contains(r[0].ToString()))
                                CreateValueContainer<DateTimePicker>(r[0].ToString());
                            else if (_columnsTypes[_tableName].DateColumns.Contains(r[0].ToString()))
                                CreateValueContainer<DatePicker>(r[0].ToString());
                            else if (_columnsTypes[_tableName].TimeColumns.Contains(r[0].ToString()))
                                CreateValueContainer<TimePicker>(r[0].ToString());
                            else
                                CreateValueContainer<TextBox>(r[0].ToString());
                        }

                        if (!_isInsert)
                        {
                            editDataPageContent.Children[editDataPageContent.Children.Count - 1].GetType().GetProperty("IsEnabled").SetValue(editDataPageContent.Children[editDataPageContent.Children.Count - 1], !(bool)r[4]);

                            if (data.Rows[0][i] is byte[])
                            {
                                int length = BitConverter.ToInt32(((byte[])data.Rows[0][i]).Take(4).ToArray(), 0);
                                editDataPageContent.Children[editDataPageContent.Children.Count - 1].GetType().GetProperty("Text").SetValue(editDataPageContent.Children[editDataPageContent.Children.Count - 1], Encoding.Default.GetString((byte[])data.Rows[0][i], 4, length));
                            }
                            else
                            {
                                UIElement uie = editDataPageContent.Children[editDataPageContent.Children.Count - 1];
                                if (uie is DateTimePicker || uie is TimePicker)
                                    uie.GetType().GetProperty("SelectedDateTime").SetValue(editDataPageContent.Children[editDataPageContent.Children.Count - 1], DateTime.Parse(data.Rows[0][i].ToString()));
                                else if (uie is DatePicker)
                                    ((DatePicker)uie).SelectedDate = DateTime.Parse(data.Rows[0][i].ToString());
                                else
                                    uie.GetType().GetProperty("Text").SetValue(editDataPageContent.Children[editDataPageContent.Children.Count - 1], data.Rows[0][i].ToString().Replace(',', '.'));
                            }
                        }

                        if (string.Equals((string)r[1], "image") || r[1].ToString().Contains("binary"))
                        {
                            Button butt = new Button
                            {
                                Width = 40,
                                Height = 40,
                                Margin = new Thickness(100, 0, 0, 0),
                                Content = new Image
                                {
                                    Stretch = Stretch.Uniform
                                }
                            };
                            AnimationBehavior.SetSourceUri((Image)butt.Content, new Uri("pack://application:,,,/Resources/Icons/download.gif"));
                            Grid.SetColumn(butt, 1);
                            Grid.SetRow(butt, editDataPageContent.RowDefinitions.Count - 1);
                            editDataPageContent.Children.Add(butt);

                            //Выбор файла
                            butt.Click += new RoutedEventHandler((object o, RoutedEventArgs rea) =>
                            {
                                SWF.FileDialog fd = new SWF.OpenFileDialog();
                                string filter = null;
                                _filesTypes.ToList().ForEach((string n) => filter += $"{n}|*.{n}|");
                                fd.Filter = filter.Substring(0, filter.Length - 1);
                                fd.ShowDialog();
                                editDataPageContent.Children[editDataPageContent.Children.IndexOf(butt) - 1].GetType().GetProperty("Text").SetValue(editDataPageContent.Children[editDataPageContent.Children.IndexOf(butt) - 1], fd.FileName);
                            });
                        }
                        i++;
                    });

                    List<string> foreignTablesL = new List<string>();
                    if (_foreignTables.ContainsKey(_tableName) && _useFT)
                        foreignTablesL = _foreignTables[_tableName].ToList();
                    else
                        foreignTables.Rows.Cast<DataRow>().Where(r => _useFT).ToList().ForEach((DataRow r) => foreignTablesL.Add(r[0].ToString()));

                    foreignTablesL.ForEach((string t) =>
                    {
                        List<string> columnsNames = new List<string>();
                        tablesPieces.Items.Add(new TabItem
                        {
                            Header = _tableSynonyms.ContainsKey(t) ? _tableSynonyms[t].TableSynonym : t,
                            Style = (Style)Application.Current.Resources["tcStyle"],
                            Content = new Grid() { Margin = new Thickness(10) }
                        });

                        _columnsTypes.Add(t, new Columns());

                        switch (App.SystemConfigs.SelectedDBMS)
                        {
                            case DBMS.MSSQL:
                                {
                                    columns = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_columns_table '{t}'", App.SystemConfigs.ConnectionStr));
                                    foreignColumns = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_foreign_columns '{t}'", App.SystemConfigs.ConnectionStr));
                                    break;
                                }
                            case DBMS.MySQL:
                                {
                                    columns = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter($"call get_columns_table ('{t}')", App.SystemConfigs.ConnectionStr));
                                    foreignColumns = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter($"call get_foreign_columns '{t}'", App.SystemConfigs.ConnectionStr));
                                    break;
                                }
                        }

                        for (i = 0; i < columns.Rows.Count; i++)
                            columns.Rows.Cast<DataRow>().Where(r => !Equals(r, columns.Rows[i]) && string.Equals(r[0].ToString(), columns.Rows[i][0].ToString())).ToList().ForEach((DataRow r) => columns.Rows.Remove(r));

                        columns.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                        {
                            columnsNames.Add(r[0].ToString());

                            if (!string.IsNullOrEmpty(r[2].ToString()))
                                _columnsTypes[t].StrColumns.Add(r[0].ToString());
                            else if (string.Equals(r[1].ToString(), "datetime"))
                                _columnsTypes[t].DateTimeColumns.Add(r[0].ToString());
                            else if (string.Equals(r[1].ToString(), "date"))
                                _columnsTypes[t].DateColumns.Add(r[0].ToString());
                            else if (string.Equals(r[1].ToString(), "time"))
                                _columnsTypes[t].TimeColumns.Add(r[0].ToString());
                            else if (r[1].ToString().Contains("binary") || string.Equals(r[1].ToString(), "image"))
                                _columnsTypes[t].BinaryColumns.Add(r[0].ToString());

                        });
                        foreignColumns.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                        {
                            if (!_columnsTypes[t].ForeignColumns.ContainsKey(r[1].ToString()))
                            {
                                _columnsTypes[t].ForeignColumns.Add(r[1].ToString(), new Dictionary<string, string>());
                                _columnsTypes[t].ForeignColumns[r[1].ToString()].Add(r[0].ToString(), r[2].ToString());
                            }
                        });

                        DataTable data1 = new DataTable();
                        if (_isInsert)
                            data1 = null;
                        else
                        {
                            string where = null;
                            foreignColumns.Rows.Cast<DataRow>().Where(r => _columnsTypes[t].ForeignColumns[_tableName].ContainsKey(r[0].ToString())).ToList().ForEach((DataRow r) => where += _columnsTypes[t].StrColumns.Contains(r[0].ToString()) ? $"{r[0]} = '{data.Rows[0][data.Columns.IndexOf(data.Columns.Cast<DataColumn>().Where(c => string.Equals(c.ColumnName, r[2].ToString())).ToArray()[0])]}' and " : $"{r[0]} = {data.Rows[0][data.Columns.IndexOf(data.Columns.Cast<DataColumn>().Where(c => string.Equals(c.ColumnName, r[2].ToString())).ToArray()[0])]} and ");
                            where = where.Substring(0, where.Length - 5);

                            switch (App.SystemConfigs.SelectedDBMS)
                            {
                                case DBMS.MSSQL:
                                    {
                                        data1 = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter(
                                            $"SELECT " +
                                            "* " +
                                            $"FROM {t} " +
                                            $"WHERE {where}", App.SystemConfigs.ConnectionStr));
                                        break;
                                    }
                                case DBMS.MySQL:
                                    {
                                        break;
                                    }
                            }
                        }
                        GetTable(t, (Grid)((TabItem)tablesPieces.Items[tablesPieces.Items.Count - 1]).Content, columnsNames.ToArray(), false, true, false, data1);
                    });

                    if (_additionalButtons != null)
                        _additionalButtons.ToList().ForEach((KeyValuePair<string, RoutedEventHandler> kvp) =>
                        {
                            object prop = typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic).Where(p => kvp.Key.Contains(p.Name)).ToArray()[0];
                            Button butt = new Button
                            {
                                Height = 30,
                                Width = 30,
                                Content = new Image
                                {
                                    Stretch = Stretch.Uniform,
                                    Source = kvp.Key.Contains(".gif") ? null : Imaging.CreateBitmapSourceFromHBitmap(
                                       (IntPtr)prop.GetType().GetMethod("GetHbitmap").Invoke(prop, new object[0]),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                                }
                            };
                            if (((Image)butt.Content).Source == null)
                                AnimationBehavior.SetSourceUri((Image)butt.Content, new Uri(kvp.Key));

                            butt.Click += kvp.Value;
                            additionalButtons.Items.Add(butt);
                        });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                }
            }
            else
            {
                filterTables.Items.Clear();
                _dataTable = null;
                _columnsTypes.Clear();
                editDataPageContent.Children.Clear();
                editDataPageContent.RowDefinitions.Clear();
                tablesPieces.Items.Clear();
                additionalButtons.Items.Clear();
                _rowsOfTables.Clear();
            }
        }

        private void CreateValueContainer<T>(string colName, string tableName = null, string primaryColName = null) where T : Control, new()
        {
            T container = new T
            {
                
                FontFamily = new FontFamily("pack://application:,,,/Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Height = 30,
                Width = 200,
                ToolTip = colName
            };
            Grid.SetColumn(container, 1);
            Grid.SetRow(container, editDataPageContent.RowDefinitions.Count - 1);
            editDataPageContent.Children.Add(container);

            if (container is ComboBox)
            {
                (container as ComboBox).IsEditable = true;
                DataTable t = new DataTable();

                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter
                                (
                                    "SELECT " +
                                    $"{primaryColName} " +
                                    "FROM " +
                                    $"{tableName}", App.SystemConfigs.ConnectionStr
                                ));
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            break;
                        }
                }
                new List<DataRow>(t.AsEnumerable()).ForEach((DataRow r) => (container as ComboBox).Items.Add(r[0]));
            }
            else if (container is TextBox)
                container.Style = (Style)Application.Current.Resources["tbStyle"];
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (tablesPieces.SelectedIndex != -1)
            {
                string header = ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString();
                _tableSynonyms.Where(kvp => Equals(kvp.Value.TableSynonym, header)).ToList().ForEach((KeyValuePair<string, TablesSynonyms> kvp) => header = kvp.Key);

                string tableName = ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString();
                DataTable t = new DataTable
                {
                    TableName = _tableSynonyms.ContainsKey(tableName) ? tableName : _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToArray()[0].Key
                };
                UIElement control = ((Grid)((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Content).Children.Cast<UIElement>().Where(c => c is ScrollViewer).ToArray()[0];
                ColumnDefinitionCollection columnDefinitions = ((Grid)control.GetType().GetProperty("Content").GetValue(control)).ColumnDefinitions;
                columnDefinitions.Cast<ColumnDefinition>().Where(cd =>
                {
                    if (_editable)
                        return columnDefinitions.IndexOf(cd) < columnDefinitions.Count - 1;
                    else
                        return true;
                }).ToList().ForEach((ColumnDefinition cd) => t.Columns.Add(new DataColumn()));
                t.Rows.Add(new object[t.Columns.Count]);
                AddingRows(t, (ScrollViewer)control, false);
            }
        }

        private void DropRow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveRow_Click(object sender, RoutedEventArgs e)
        {
            using (var conn = GetConnectionObj<SqlConnection>())
            {
                try
                {
                    Query(new SqlCommand("BEGIN TRAN", conn));

                    if (SaveRow(editDataPageContent, _columnsTypes.AsQueryable().ToArray()[0], conn))
                    {
                        tablesPieces.Items.Cast<TabItem>().ToList().ForEach((TabItem ti) => _columnsTypes.Where(k => !Equals(k, _tableName)).ToList().ForEach((KeyValuePair<string, Columns> kvp) =>
                        {
                            UIElement control = ((Grid)ti.Content).Children.Cast<UIElement>().Where(c => c is ScrollViewer).ToArray()[0];
                            if (_tableSynonyms.ContainsKey(kvp.Key))
                            {
                                if (string.Equals(_tableSynonyms[kvp.Key].TableSynonym, ti.Header.ToString()))
                                    for (int i = 0; i < ((Grid)((ScrollViewer)control).Content).RowDefinitions.Count; i++)
                                        SaveRow((Grid)((ScrollViewer)control).Content, i, kvp, conn);
                            }
                            else if (Equals(kvp.Key, ti.Header))
                                for (int i = 0; i < ((Grid)((ScrollViewer)control).Content).RowDefinitions.Count; i++)
                                    SaveRow((Grid)((ScrollViewer)control).Content, i, kvp, conn);
                        }));

                        tablesPieces.Items.Cast<TabItem>().ToList().ForEach((TabItem ti) => DeleteRow((Grid)((ScrollViewer)((Grid)ti.Content).Children[((Grid)ti.Content).Children.Count - 1]).Content, GetConnectionObj<SqlConnection>()));

                        Query(new SqlCommand("COMMIT TRAN", conn));
                        SideMessage.Show(Content as Grid, "Запись сохранена!", SideMessage.Type.Info, MahApps.Metro.Controls.Position.Right);
                        menu.SelectedIndex = 0;
                    }
                    else
                        Query(new SqlCommand("ROLLBACK TRAN", conn));
                }
                catch (Exception err)
                {
                    Query(new SqlCommand("ROLLBACK TRAN", conn));
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                }
            }
        }

        private bool SaveRow<T>(Grid container, KeyValuePair<string, Columns> kvp, T conn) where T : DbConnection
        {
            string values = null;
            List<DbParameter> parameters = new List<DbParameter>();
            
            int i = 0;
            container.Children.Cast<UIElement>().Where(c => c.GetType().GetProperty("Text") != null || c.GetType().GetProperty("SelectedDate") != null || c.GetType().GetProperty("SelectedDateTime") != null).ToList().ForEach((UIElement c) =>
            {
                UIElement tbb = c is Border ? ((Border)c).Child : c;
                PropertyInfo property;

                if (tbb is DatePicker)
                    property = tbb.GetType().GetProperty("SelectedDate");
                else if (tbb is DateTimePicker || tbb is TimePicker)
                    property = tbb.GetType().GetProperty("SelectedDateTime");
                else
                    property = tbb.GetType().GetProperty("Text");

                if (_isInsert)
                {
                    if (string.IsNullOrEmpty(property.GetValue(tbb).ToString()))
                        values += "NULL,";
                    else if (kvp.Value.BinaryColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)))
                    {
                        if (File.Exists(property.GetValue(tbb).ToString()))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (BinaryReader input = new BinaryReader(File.Open(property.GetValue(tbb).ToString(), FileMode.Open)))
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);

                                parameters.Add(GetParameter<SqlParameter>($"@p{i}", BitConverter.GetBytes((property.GetValue(tbb).ToString()).Length).Concat(Encoding.Default.GetBytes(property.GetValue(tbb).ToString()).Concat(GetFileType(property.GetValue(tbb).ToString()).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray()));
                                values += $"@p{i},";

                                i++;
                            }
                        }
                        else
                            values += "NULL,";
                    }
                    else
                        values += kvp.Value.StrColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.DateTimeColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.DateColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.TimeColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) ? $"'{property.GetValue(tbb)}'," : $"{property.GetValue(tbb)},";
                }
                else if (tbb.IsEnabled)
                {
                    if (string.IsNullOrEmpty(property.GetValue(tbb).ToString()))
                        values += $"{tbb.GetType().GetProperty("ToolTip").GetValue(tbb)} = NULL,";
                    else if (kvp.Value.BinaryColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)))
                    {
                        if (File.Exists(property.GetValue(tbb).ToString()))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (BinaryReader input = new BinaryReader(File.Open(property.GetValue(tbb).ToString(), FileMode.Open)))
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);

                                parameters.Add(GetParameter<SqlParameter>($"@p{i}", BitConverter.GetBytes((property.GetValue(tbb).ToString()).Length).Concat(Encoding.Default.GetBytes(property.GetValue(tbb).ToString()).Concat(GetFileType(property.GetValue(tbb).ToString()).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray()));
                                values += $"{tbb.GetType().GetProperty("ToolTip").GetValue(tbb)} = @p{i},";

                                i++;
                            }
                        }
                        else
                            values += $"{tbb.GetType().GetProperty("ToolTip").GetValue(tbb)} = NULL,";
                    }
                    else
                        values += kvp.Value.StrColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.DateTimeColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.DateColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) || kvp.Value.TimeColumns.Contains(tbb.GetType().GetProperty("ToolTip").GetValue(tbb)) ? $"{tbb.GetType().GetProperty("ToolTip").GetValue(tbb)} = '{property.GetValue(tbb)}'," : $"{tbb.GetType().GetProperty("ToolTip").GetValue(tbb)} = {property.GetValue(tbb)},";
                }
            });
            values = values.Substring(0, values.Length - 1);

            try
            {
                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            SqlCommand comm = _isInsert ? new SqlCommand($"INSERT INTO {kvp.Key} values ({values})", conn as SqlConnection) : new SqlCommand($"UPDATE {kvp.Key} SET {values} WHERE {_where}", conn as SqlConnection);
                            parameters.ForEach((DbParameter p) => comm.Parameters.Add(p));
                            Query(comm);
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            Query(new MySqlCommand($"INSERT INTO {kvp.Key} values ({values})", conn as MySqlConnection));
                            break;
                        }
                }
                return true;
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                return false;
            }
        }
        private bool SaveRow<T>(Grid container, int rowIndex, KeyValuePair<string, Columns> kvp, T conn) where T : DbConnection
        {
            string values = null, where = null;
            List<DbParameter> parameters = new List<DbParameter>();
            bool isUpdate = false;

            int i = 0;
            container.Children.Cast<Border>().Where(c => ((FrameworkElement)c.Child).ToolTip != null && Grid.GetRow(c) == rowIndex).ToList().ForEach((Border c) =>
            {
                isUpdate = _rowsOfTables.ContainsKey(kvp.Key);
                isUpdate = isUpdate ? _rowsOfTables[kvp.Key].ContainsKey(c.Child) : isUpdate;

                if (!isUpdate)
                {
                   if (kvp.Value.BinaryColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)))
                    {
                        if (File.Exists((string)((Button)((Grid)((Label)c.Child).Content).Children[0]).ToolTip))
                        {
                            string fileName = (string)((Button)((Grid)((Label)c.Child).Content).Children[0]).ToolTip;
                            byte[] buffer = new byte[16 * 1024];
                            using (BinaryReader input = new BinaryReader(File.Open(fileName, FileMode.Open)))
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);

                                parameters.Add(GetParameter<SqlParameter>($"@p{i}", BitConverter.GetBytes(fileName.Length).Concat(Encoding.Default.GetBytes(fileName).Concat(GetFileType(fileName).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray()));
                                values += $"@p{i},";

                                i++;
                            }
                        }
                        else
                            values += "NULL,";
                    }
                    else if (string.IsNullOrEmpty((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)))
                        values += "NULL,";
                    else
                        values += kvp.Value.StrColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.DateTimeColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.DateColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.TimeColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) ? $"'{c.Child.GetType().GetProperty("Text").GetValue(c.Child)}'," : $"{c.Child.GetType().GetProperty("Text").GetValue(c.Child)},";
                }
                else if (c.IsEnabled)
                {
                    if (string.IsNullOrEmpty((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)))
                        values += $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = NULL,";
                    else if (kvp.Value.BinaryColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)))
                    {
                        if (File.Exists((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (BinaryReader input = new BinaryReader(File.Open((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child), FileMode.Open)))
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);

                                parameters.Add(GetParameter<SqlParameter>($"@p{i}", BitConverter.GetBytes(((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)).Length).Concat(Encoding.Default.GetBytes((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)).Concat(GetFileType((string)c.Child.GetType().GetProperty("Text").GetValue(c.Child)).Concat(ms.ToArray()).ToArray()).ToArray()).ToArray()));
                                values += $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = @p{i},";

                                i++;
                            }
                        }
                        else
                            values += $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = NULL,";
                    }
                    else
                        values += kvp.Value.StrColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.DateTimeColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.DateColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) || kvp.Value.TimeColumns.Contains(c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)) ? $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = '{c.Child.GetType().GetProperty("Text").GetValue(c.Child)}'," : $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = {c.Child.GetType().GetProperty("Text").GetValue(c.Child)},";

                    string colName = c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child).ToString(), value;

                    value = _rowsOfTables[kvp.Key][c.Child];
                    where += kvp.Value.StrColumns.Contains(colName) || kvp.Value.DateTimeColumns.Contains(colName) || kvp.Value.DateColumns.Contains(colName) || kvp.Value.TimeColumns.Contains(colName) ? $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = '{value}' and " : $"{c.Child.GetType().GetProperty("ToolTip").GetValue(c.Child)} = {value} and ";
                }
            });
            values = values.Substring(0, values.Length - 1);
            if (!string.IsNullOrEmpty(where))
                where = where.Substring(0, where.Length - 4);

            try
            {
                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            SqlCommand comm = isUpdate ? new SqlCommand($"UPDATE {kvp.Key} SET {values} WHERE {where}", conn as SqlConnection) : new SqlCommand($"INSERT INTO {kvp.Key} values ({values})", conn as SqlConnection);
                            parameters.ForEach((DbParameter p) => comm.Parameters.Add(p));
                            Query(comm);
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            Query(new MySqlCommand($"INSERT INTO {kvp.Key} values ({values})", conn as MySqlConnection));
                            break;
                        }
                }
                return true;
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, MahApps.Metro.Controls.Position.Right);
                return false;
            }
        }

        private T GetParameter<T>(string parameterName, object value) where T : DbParameter, new() => new T
        {
            ParameterName = parameterName,
            Value = value
        };

        private byte[] GetFileType(string fileName)
        {
            string fileType = new FileInfo(fileName).Extension;
            return BitConverter.GetBytes(_filesTypes.ToList().IndexOf(fileType.Substring(1, fileType.Length - 1)));
        }
    }
}