using System;
using System.Reflection;
using System.Windows.Markup;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using SWF = System.Windows.Forms;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using XamlAnimatedGif;
using pdf = Syncfusion.Pdf;
using xps = Syncfusion.XPS;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using System.Printing;

namespace GasStation
{
    /// <summary>
    /// Контент приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //AddHandler(MouseDownEvent, new MouseButtonEventHandler(UpPanel_MouseDown), true);

            content.Background = new SolidColorBrush(App.SystemConfigs.MainColor);
            menu.Background = new SolidColorBrush(App.SystemConfigs.FunctionsColor);

            upPanel.AddHandler(MouseDownEvent, new MouseButtonEventHandler(UpPanel_MouseDown), true);

            //Загрузка синонимов таблиц
            LoadSynonyms(new DirectoryInfo(".").FullName);

            //Вторичные таблицы
            _foreignTables.Add("fuels", new string[] { "fuels_prices" });
            _foreignTables.Add("products", new string[0]);
            _foreignTables.Add("stuffers", new string[0]);
            _foreignTables.Add("fuel_suppliers", new string[0]);
            _foreignTables.Add("products_suppliers", new string[0]);
            _foreignTables.Add("gas_columns", new string[] { "gas_columns_fuel_types" });
            _foreignTables.Add("products_types", new string[0]);

            DataTable t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_login_full_name '{App.SystemConfigs.Login}'", App.SystemConfigs.ConnectionStr));
            _userFullName = t.Rows.Count > 0 ? (string)t.Rows[0][0] : null;
        }

        private void LoadSynonyms(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            directories.ToList().ForEach((string dir) => LoadSynonyms(dir));

            string[] files = Directory.GetFiles(path, "*.json");
            files.ToList().ForEach((string f) =>
            {
                JObject obj;
                string fn;
                using (StreamReader sr = new StreamReader(File.Open(f, FileMode.Open), Encoding.Default))
                {
                    fn = new FileInfo(f).Name;
                    fn = fn.Substring(0, fn.IndexOf('.'));
                    _tableSynonyms.Add(fn, new TablesSynonyms());
                    obj = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                }

                try
                {
                    _tableSynonyms[fn].TableSynonym = obj.Properties().Where(p => p.Name == "TableSynonym").ToArray()[0].Value.ToString();
                    ((JObject)obj.Properties().Where(p => p.Name != "TableSynonym").ToArray()[0].Value).Properties().ToList().ForEach((JProperty p) => _tableSynonyms[fn].ColumnsSynonyms.Add(p.Name, p.Value.ToString()));
                }
                catch (Exception)
                {
                    _tableSynonyms.Remove(fn);
                    return;
                }
            });
        }
        
        private void Close_MouseDown(object sender, MouseButtonEventArgs e) => SideMessage.Show(Content as Grid, "Завершить работу?", SideMessage.Type.Warning, Position.Right, new SideMessage.Behaviors
        {
            Yes = (object s, RoutedEventArgs reh) =>
            {
                _openedFiles.ForEach((Process p) =>
                {
                    p.Kill();
                    p.Dispose();
                });
                Thread.Sleep(3000);
                Directory.GetFiles(new DirectoryInfo(".").FullName).Where(fn => fn.Contains(_cacheFileName)).ToList().ForEach((string fn) => File.Delete(fn));
                Application.Current.Shutdown();
            }
        });

        private void Minimize_MouseDown(object sender, MouseButtonEventArgs e) => Application.Current.Windows.Cast<Window>().ToList().ForEach((Window win) => win.WindowState = WindowState.Minimized);
        private void UpPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private async void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (menu.SelectedIndex != -1)
            {
                if (menu.SelectedItem == gasColumns)
                {
                    App.SystemConfigs.ConnectionStr = "GasStation";
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStation<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStation<ListViewItem>));
                }
                else if (menu.SelectedItem == shop)
                {
                    App.SystemConfigs.ConnectionStr = "GasStationShop";
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
                }
                else if (menu.SelectedItem == staffers)
                {
                    App.SystemConfigs.ConnectionStr = "GasStation";
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfStuffers<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfStuffers<ListViewItem>));
                }
            }
            menu.SelectedIndex = -1;
        }

        private void GasColumnsInfo(object sender, MouseButtonEventArgs e)
        {

        }

        //Обработка select запросов
        private TMarshalByValueComponent QuerySelect<TDbDataAdapter, TMarshalByValueComponent>(TDbDataAdapter ad) where TDbDataAdapter : DbDataAdapter where TMarshalByValueComponent : MarshalByValueComponent, new()
        {
            using (ad)
            {
                TMarshalByValueComponent t = new TMarshalByValueComponent();
                if (t is DataTable)
                    ad.Fill(t as DataTable);
                else
                    ad.Fill(t as DataSet);
                return t;
            }
        }

        private void Query<T>(T comm) where T : DbCommand
        {
            using (comm)
                comm.ExecuteNonQuery();
        }
        
        private T GetConnectionObj<T>() where T : DbConnection, new()
        {
            T conn = new T
            {
                ConnectionString = App.SystemConfigs.ConnectionStr
            };
            conn.Open();
            return conn;
        }

        private void GenerateTable(Grid content, string tableName, string[] columns, bool readOnly, bool editable, bool savePdf, DataTable data = null, RoutedEventHandler editClick = null)
        {
            if (_dataTable == null)
                _dataTable = data;

            if (this.content.Children.Contains(_savePdfButton))
                this.content.Children.Remove(_savePdfButton);

            _editable = editable;
            _savePdf = savePdf;
            content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });

            List<string> columnsOfData = new List<string>();
            if (data != null)
            {
                _columnsTypes.Add(tableName, new Columns());
                data.Columns.Cast<DataColumn>().ToList().ForEach((DataColumn c) =>
                {
                    if (c.DataType == typeof(string))
                        _columnsTypes[tableName].StrColumns.Add(c.ColumnName);
                    else if (c.DataType == typeof(DateTime))
                        _columnsTypes[tableName].DateColumns.Add(c.ColumnName);
                    else if (c.DataType == typeof(byte[]))
                        _columnsTypes[tableName].BinaryColumns.Add(c.ColumnName);

                    columnsOfData.Add(c.ColumnName);
                });
                data.TableName = tableName;
            }
            else
                columnsOfData = columns.ToList();

            AddingColumns(content, tableName, columnsOfData.ToArray(), true, true);

            content.RowDefinitions.Add(new RowDefinition());
            ScrollViewer sv = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = double.NaN,
                Height = double.NaN,
                FontFamily = new FontFamily("/Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            Grid.SetColumnSpan(sv, content.ColumnDefinitions.Count);
            Grid.SetRow(sv, 1);
            sv.Content = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = double.NaN,
                Height = double.NaN
            };

            content.Children.Add(sv);
 
            AddingColumns((Grid)sv.Content, tableName, columnsOfData.ToArray(), false, readOnly);

            if (data != null)
                AddingRows(data, sv, readOnly, editClick);

            if (savePdf)
            {
                tableName = _tableSynonyms.ContainsKey(tableName) ? _tableSynonyms[tableName].TableSynonym : tableName;

                _savePdfButton = new Button
                {
                    Height = 60,
                    Width = 60,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(10),
                    Content = new Image
                    {
                        Source = Imaging.CreateBitmapSourceFromHBitmap(
                                               Properties.Resources.save_pdf.GetHbitmap(),
                                               IntPtr.Zero,
                                               Int32Rect.Empty,
                                               BitmapSizeOptions.FromEmptyOptions()),
                        Stretch = Stretch.Uniform,
                        ToolTip = "Сохранить в PDF"
                    }
                };

                _savePdfButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
                {
                    SWF.SaveFileDialog sfd = new SWF.SaveFileDialog
                    {
                        Filter = "XPS|*.xps"
                    };
                    sfd.ShowDialog();

                    if (!string.IsNullOrEmpty(sfd.FileName))
                    {
                        ScrollViewer sv1;
                        FixedDocument fixedDoc = new FixedDocument();
                        PageContent pageContent = new PageContent();
                        FixedPage fixedPage = new FixedPage();

                        PrintDialog printDlg = new PrintDialog();
                        Size pageSize = new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight - 100);

                        Grid visual = tablePage;
                        sv1 = (ScrollViewer)tablePage.Parent;
                        sv1.Content = null;

                        fixedPage.Children.Add(visual);
                        ((IAddChild)pageContent).AddChild(fixedPage);

                        fixedDoc.Pages.Add(pageContent);

                        // write to PDF file
                        using (XpsDocument xpsd = new XpsDocument(sfd.FileName, FileAccess.ReadWrite))
                            XpsDocument.CreateXpsDocumentWriter(xpsd).Write(fixedDoc);

                        xps.XPSToPdfConverter converter = new xps.XPSToPdfConverter();
                        pdf.PdfDocument document = converter.Convert(sfd.FileName);
                        FileInfo fi = new FileInfo(sfd.FileName);

                        if (File.Exists($"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.IndexOf('.'))}.pdf"))
                            File.Delete($"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.IndexOf('.'))}.pdf");

                        document.Save($"{fi.DirectoryName}\\{fi.Name.Substring(0, fi.Name.IndexOf('.'))}.pdf");
                        document.Close(true);
                        File.Delete(sfd.FileName);

                        if (tablePage.Parent != sv1)
                        {
                            ((FixedPage)tablePage.Parent).Children.Remove(tablePage);
                            sv1.Content = tablePage;
                        }
                    }
                });
                this.content.Children.Add(_savePdfButton);
            }
        }

        private void AddingRows(DataTable data, ScrollViewer sv, bool readOnly, RoutedEventHandler editClick = null) => data.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
        {
            ((Grid)sv.Content).RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });

            int i = 0;
            r.ItemArray.ToList().ForEach((object o) =>
            {
                Border b = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(2, 2, 2, 0),
                };
                b.MouseMove += new MouseEventHandler((object sender, MouseEventArgs e) => ((Grid)sv.Content).Children.Cast<Border>().Where(uie => Grid.GetRow(uie) == Grid.GetRow(b)).ToList().ForEach((Border uie) => uie.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D58E40"))));
                b.MouseLeave += new MouseEventHandler((object sender, MouseEventArgs e) => ((Grid)sv.Content).Children.Cast<Border>().Where(uie => Grid.GetRow(uie) == Grid.GetRow(b)).ToList().ForEach((Border uie) => uie.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6BC8E"))));

                string tableName = tablesPieces.Items.Count > 0 ? tablesPieces.Items.Cast<TabItem>().Where(ti => Equals(ti.Content, sv.Parent)).ToArray()[0].Header.ToString() : data.TableName,
                colName = tablesPieces.Items.Count > 0 ? ((TextBlock)((Border)((Grid)sv.Parent).Children[i]).Child).Text : data.Columns[i].ColumnName;

                _tableSynonyms.Where(kvp => string.Equals(tableName, kvp.Value.TableSynonym)).ToList().ForEach((KeyValuePair<string, TablesSynonyms> kvp) =>
                {
                    tableName = kvp.Key;
                    kvp.Value.ColumnsSynonyms.Where(kvp1 => string.Equals(colName, kvp1.Value)).ToList().ForEach((KeyValuePair<string, string> kvp1) => colName = kvp1.Key);
                });
                if (_columnsTypes.ContainsKey(tableName))
                    _columnsTypes[tableName].ForeignColumns.Where(kvp => kvp.Value.ContainsKey(colName)).ToList().ForEach((KeyValuePair<string, Dictionary<string, string>> kvp) =>
                    {
                        b.IsEnabled = editDataPageContent.Children.Cast<FrameworkElement>().Where(el => el.ToolTip != null).Where(el => el.ToolTip.ToString() == kvp.Value[colName]).Count() < 1;
                        b.Child = kvp.Value.ContainsKey(colName) ? SetCellContainer<ComboBox>(tableName, colName, kvp.Key, kvp.Value[colName]) : null;
                    });

                if (b.Child == null)
                {
                    if (_columnsTypes.ContainsKey(tableName))
                    {
                        if (_columnsTypes[tableName].BinaryColumns.Contains(colName))
                        {
                            if (readOnly)
                                b.Child = SetCellContainer<TextBlock>(tableName, colName);
                            else
                            {
                                b.Child = SetCellContainer<Label>(tableName, colName);
                                ((Label)b.Child).Content = new Grid();

                                int length = string.IsNullOrEmpty(o.ToString()) ? 0 : BitConverter.ToInt32(((byte[])o).Take(4).ToArray(), 0);

                                Button butt = new Button
                                {
                                    Width = 40,
                                    Height = 40,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Content = new Image
                                    {
                                        Stretch = Stretch.Uniform
                                    },
                                    ToolTip = length == 0 ? null : Encoding.Default.GetString((byte[])o, 4, length)
                                };
                                AnimationBehavior.SetSourceUri((Image)butt.Content, new Uri("pack://application:,,,/Resources/Icons/download.gif"));

                                //Выбор файла
                                butt.Click += new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                                {
                                    SWF.FileDialog fd = new SWF.OpenFileDialog();
                                    string filter = null;
                                    _filesTypes.ToList().ForEach((string n) => filter += $"{n}|*.{n}|");
                                    fd.Filter = filter.Substring(0, filter.Length - 1);
                                    fd.ShowDialog();
                                    butt.ToolTip = string.IsNullOrEmpty(fd.FileName) ? null : fd.FileName;
                                });
                                ((Grid)((Label)b.Child).Content).Children.Add(butt);

                                Button butt1 = new Button
                                {
                                    Width = 40,
                                    Height = 40,
                                    Margin = new Thickness(80, 0, 0, 0),
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Content = new Image
                                    {
                                        Stretch = Stretch.Uniform
                                    }
                                };
                                AnimationBehavior.SetSourceUri((Image)butt1.Content, new Uri("pack://application:,,,/Resources/Icons/cancel.gif"));

                                butt1.Click += new RoutedEventHandler((object obj, RoutedEventArgs rea) => ((Button)((Grid)((Label)b.Child).Content).Children[0]).ToolTip = null);
                                ((Grid)((Label)b.Child).Content).Children.Add(butt1);
                            }
                        }
                        else
                        {
                            if (readOnly)
                                b.Child = SetCellContainer<TextBlock>(tableName, colName);
                            else
                            {
                                if (_columnsTypes[tableName].DateTimeColumns.Contains(colName))
                                    b.Child = SetCellContainer<DateTimePicker>(tableName, colName);
                                else if (_columnsTypes[tableName].DateColumns.Contains(colName))
                                    b.Child = SetCellContainer<DatePicker>(tableName, colName);
                                else if (_columnsTypes[tableName].TimeColumns.Contains(colName))
                                    b.Child = SetCellContainer<TimePicker>(tableName, colName);
                                else
                                    b.Child = SetCellContainer<TextBox>(tableName, colName);
                            }
                        }
                    }
                    else
                        b.Child = readOnly ? SetCellContainer<TextBlock>(tableName, colName) : SetCellContainer<TextBox>(tableName, colName);
                }

                if (o is byte[])
                {
                    if (tablesPieces.Items.Count < 1)
                    {
                        b.Child.MouseDown += new MouseButtonEventHandler(OpenFile);
                        _binaryDataControls.Add(b.Child, (byte[])o);
                        b.Child.GetType().GetProperty("Text").SetValue(b.Child, "<Открыть файл>");
                        b.Child.GetType().GetProperty("Cursor").SetValue(b.Child, Cursors.Hand);
                    }
                }
                else if (!string.IsNullOrEmpty(o.ToString()))
                {
                    if (b.Child is DateTimePicker || b.Child is TimePicker)
                        b.Child.GetType().GetProperty("SelectedDateTime").SetValue(b.Child, DateTime.Parse(o.ToString()));
                    else if (b.Child is DatePicker)
                        ((DatePicker)b.Child).SelectedDate = DateTime.Parse(o.ToString());
                    else
                    {
                        string value;
                        if (_columnsTypes.ContainsKey(tableName))
                            value = _columnsTypes[tableName].StrColumns.Contains(colName) ? o.ToString() : o.ToString().Replace(',', '.');
                        else
                            value = o.ToString();
                        b.Child.GetType().GetProperty("Text").SetValue(b.Child, value);
                    }
                }

                ((Grid)sv.Content).Children.Add(b);
                Grid.SetRow(b, ((Grid)sv.Content).RowDefinitions.Count - 1);
                Grid.SetColumn(b, i);

                if (!b.IsEnabled)
                {
                    FrameworkElement fe = editDataPageContent.Children.Cast<FrameworkElement>().Where(uie => uie.ToolTip != null).Where(uie => string.Equals(_columnsTypes[tableName].ForeignColumns[_tableName][colName], uie.ToolTip.ToString())).ToArray()[0];
                    b.Child.GetType().GetProperty("Text").SetValue(b.Child, fe.GetType().GetProperty("Text").GetValue(fe));
                }

                i++;

                if (i == r.ItemArray.Length && _editable)
                {
                    b = new Border
                    {
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(2, 2, 2, 0),
                        Child = SetCellContainer<Label>()
                    };
                    ((Label)b.Child).Content = new Grid { Background = null };
                    ((Label)b.Child).ToolTip = null;

                    //Редактирование записи таблицы как записи бд
                    Button butt = new Button
                    {
                        Width = 30,
                        Height = 30,
                        Content = new Image
                        {
                            Stretch = Stretch.Uniform
                        }
                    };
                    AnimationBehavior.SetSourceUri((Image)butt.Content, new Uri("pack://application:,,,/Resources/Icons/edit.gif"));
                    ((Grid)((Label)b.Child).Content).Children.Add(butt);
                    butt.Click += editClick ?? new RoutedEventHandler(async (object obj, RoutedEventArgs e) =>
                    {
                        Grid container = (Grid)((Border)((Label)((Grid)butt.Parent).Parent).Parent).Parent;
                        tableName = tablesPieces.Items.Count > 0 ? ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString() : _tableName;
                        tableName = _tableSynonyms.ContainsKey(tableName) ? tableName : _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToArray()[0].Key;
                        List<string> values = new List<string>();

                        int rIndex = Grid.GetRow((Border)((Label)((Grid)butt.Parent).Parent).Parent);
                        container.Children.Cast<UIElement>().Where(c => !(((Border)c).Child is Label) && Grid.GetRow(c) == rIndex).ToList().ForEach((UIElement c) => values.Add(((TextBlock)((Border)c).Child).Text));

                        if (_foreignTables.ContainsKey(tableName))
                            await EditPageLoad(tableName, _foreignTables[tableName].ToList(), true, false, values.ToArray());
                        else
                            await EditPageLoad(tableName, new List<string>(), true, false, values.ToArray());
                    });

                    //Удаление строки из таблицы
                    butt = new Button
                    {
                        Width = 30,
                        Height = 30,
                        Content = "-",
                        Margin = new Thickness(65, 5, 5, 5)
                    };
                    ((Grid)((Label)b.Child).Content).Children.Add(butt);
                    butt.Click += new RoutedEventHandler((object obj, RoutedEventArgs e) =>
                    {
                        Grid container = (Grid)((Border)((Label)((Grid)butt.Parent).Parent).Parent).Parent;
                        if (editDataPage.Visibility == Visibility.Visible)
                        {
                            int rIndex = Grid.GetRow((Border)((Label)((Grid)butt.Parent).Parent).Parent);
                            container.Children.Cast<UIElement>().Where(uie => Grid.GetRow(uie) == rIndex).ToList().ForEach((UIElement uie) => container.Children.Remove(uie));

                            for (i = rIndex; i < container.RowDefinitions.Count; i++)
                                container.Children.Cast<UIElement>().Where(uie => Grid.GetRow(uie) == i + 1).ToList().ForEach((UIElement uie) => Grid.SetRow(uie, i));

                            container.RowDefinitions.RemoveAt(rIndex);
                        }
                        else
                            SideMessage.Show(Content as Grid, "Удалить запись?", SideMessage.Type.Error, Position.Right, new SideMessage.Behaviors
                            {
                                Yes = new RoutedEventHandler((object sender, RoutedEventArgs rea) =>
                                {
                                    int rIndex = Grid.GetRow((Border)((Label)((Grid)butt.Parent).Parent).Parent);
                                    container.Children.Cast<UIElement>().Where(uie => Grid.GetRow(uie) == rIndex).ToList().ForEach((UIElement uie) => container.Children.Remove(uie));

                                    for (i = rIndex; i < container.RowDefinitions.Count; i++)
                                        container.Children.Cast<UIElement>().Where(uie => Grid.GetRow(uie) == i + 1).ToList().ForEach((UIElement uie) => Grid.SetRow(uie, i));

                                    container.RowDefinitions.RemoveAt(rIndex);

                                    DeleteRow(container, GetConnectionObj<SqlConnection>());
                                    ((Grid)Content).Children.Cast<UIElement>().Where(uie => uie is Flyout).ToList().ForEach(async (UIElement uie) =>
                                    {
                                        ((Flyout)uie).IsOpen = false;
                                        while (true)
                                            if (!await Task.Run(() => ((Flyout)uie).IsVisible))
                                            {
                                                ((Grid)Content).Children.Remove(uie);
                                                break;
                                            }
                                    });
                                })
                            });
                    });

                    ((Grid)sv.Content).Children.Add(b);
                    Grid.SetRow(b, ((Grid)sv.Content).RowDefinitions.Count - 1);
                    Grid.SetColumn(b, i);
                }
            });

            if (!r.ItemArray.All(obj => string.IsNullOrEmpty(obj.ToString())))
            {
                if (!_rowsOfTables.ContainsKey(data.TableName))
                    _rowsOfTables.Add(data.TableName, new Dictionary<UIElement, string>());
                ((Grid)sv.Content).Children.Cast<Border>().Where(uie => Grid.GetRow(uie) == ((Grid)sv.Content).RowDefinitions.Count - 1 && ((FrameworkElement)uie.Child).ToolTip != null).ToList().ForEach((Border uie) =>
                {
                    if (uie.Child is TextBox || uie.Child is TextBlock || uie.Child is ComboBox)
                        _rowsOfTables[data.TableName].Add(uie.Child, uie.Child.GetType().GetProperty("Text").GetValue(uie.Child).ToString());
                    else if (uie.Child is DateTimePicker || uie.Child is TimePicker)
                        _rowsOfTables[data.TableName].Add(uie.Child, uie.Child.GetType().GetProperty("SelectedDateTime").GetValue(uie.Child).ToString());
                    else if (uie.Child is DatePicker)
                        _rowsOfTables[data.TableName].Add(uie.Child, uie.Child.GetType().GetProperty("SelectedDate").GetValue(uie.Child).ToString());
                });
            }
        });

        private void AddingColumns(Grid content, string tableName, string[] columns, bool isTitle, bool readOnly) => columns.ToList().ForEach((string c) =>
        {
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            if (isTitle)
            {
                Border b = new Border
                {
                    Background = isTitle ? new SolidColorBrush(App.SystemConfigs.FunctionsColor) : null,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(2, 2, 2, 0),
                    Child = tablesPieces.Items.Count > 0 ? SetCellContainer<TextBlock>(tablesPieces.Items.Cast<TabItem>().Where(ti => Equals(ti.Content, content) || Equals(ti.Content, content.Parent.GetParentObject())).ToArray()[0].Header.ToString(), c) : SetCellContainer<TextBlock>(tableName, c)
                };
                Grid.SetColumn(b, content.ColumnDefinitions.Count - 1);

                if (_tableSynonyms.ContainsKey(tableName))
                    ((TextBlock)b.Child).Text = _tableSynonyms[tableName].ColumnsSynonyms.ContainsKey(c) ? _tableSynonyms[tableName].ColumnsSynonyms[c] : c;
                else
                    ((TextBlock)b.Child).Text = c;

                if (table.Visibility == Visibility.Hidden)
                {
                    CheckBox cb = new CheckBox
                    {
                        Content = ((TextBlock)b.Child).Text
                    };
                    cb.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => SearchTables(tablePage, tableContent, searchTables));
                    cb.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => SearchTables(tablePage, tableContent, searchTables));
                    filterTables.Items.Add(cb);
                }

                if (content.ColumnDefinitions.Count == columns.Length && _editable)
                {
                    content.Children.Add(b);
                    content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

                    b = new Border
                    {
                        Background = isTitle ? new SolidColorBrush(App.SystemConfigs.FunctionsColor) : null,
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(2, 2, 2, 0),
                        Child = tablesPieces.Items.Count > 0 ? SetCellContainer<TextBlock>(tablesPieces.Items.Cast<TabItem>().Where(ti => Equals(ti.Content, content) || Equals(ti.Content, content.Parent.GetParentObject())).ToArray()[0].Header.ToString(), c) : SetCellContainer<TextBlock>(tableName, c)
                    };
                    Grid.SetColumn(b, content.ColumnDefinitions.Count - 1);
                    ((TextBlock)b.Child).Text = "Действия";
                    ((TextBlock)b.Child).ToolTip = null;
                }

                if (content.ColumnDefinitions.Count == 1)
                    b.CornerRadius = new CornerRadius(10, 0, 0, 0);
                else if (content.ColumnDefinitions.Count >= columns.Length)
                    b.CornerRadius = new CornerRadius(0, 10, 0, 0);

                content.Children.Add(b);
            }
            else if (content.ColumnDefinitions.Count == columns.Length && _editable)
                content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
        });

        private UIElement SetCellContainer<T>(string tableName = "", string colName = "", string tableNamePK = null, string colNamePK = null) where T : FrameworkElement, new()
        {
            _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToList().ForEach((KeyValuePair<string, TablesSynonyms> kvp) => kvp.Value.ColumnsSynonyms.Where(kvp1 => string.Equals(kvp1.Value, colName)).ToList().ForEach((KeyValuePair<string, string> kvp1) => colName = kvp1.Key));
            T el = new T
            {
                ToolTip = colName
            };
            typeof(T).GetProperty("FontFamily").SetValue(el, new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"));

            if (Equals(typeof(T), typeof(TextBox)) || Equals(typeof(T), typeof(TextBlock)))
            {
                typeof(T).GetProperty("TextWrapping").SetValue(el, TextWrapping.Wrap);
                typeof(T).GetProperty("Padding").SetValue(el, new Thickness(15));
            }
            else if (Equals(typeof(T), typeof(ComboBox)))
            {
                try
                {
                    DataTable t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"SELECT {colNamePK} FROM {tableNamePK}", App.SystemConfigs.ConnectionStr));
                    t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => (el as ComboBox).Items.Add(r[0]));
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else if (!Equals(typeof(T), typeof(DateTimePicker)) && !Equals(typeof(T), typeof(DatePicker)) && !Equals(typeof(T), typeof(TimePicker)))
                typeof(T).GetProperty("Content").SetValue(el, TextWrapping.Wrap);

            return el;
        }

        private void SearchTiles_TextChanged(object sender, TextChangedEventArgs e)
        {
            (from t in tiles.Items.Cast<ContentControl>()
             select t).ToList().ForEach((ContentControl cc) => cc.Visibility = Visibility.Visible);

            (from t in tiles.Items.Cast<ContentControl>()
             select t).ToList().ForEach((ContentControl cc) =>
             {
                 if (cc.Content.ToString().Length >= searchTiles.Text.Length)
                 {
                     if (!Equals(cc.Content.ToString().Substring(0, searchTiles.Text.Length), searchTiles.Text))
                         cc.Visibility = Visibility.Hidden;
                 }
                 else
                     cc.Visibility = Visibility.Hidden;
             });
        }

        private void GetTable(string tableName, Grid content, string[] columns, bool readOnly, bool editable, bool savePdf, DataTable data = null) => GenerateTable(content, tableName, columns, readOnly, editable, savePdf, data);

        private async Task EditPageLoad(string tableName, List<string> foreignTables, bool useFT, bool isInsert, object[] keysValues = null, Dictionary<string, RoutedEventHandler> additionalButtons = null)
        {
            _tableName = tableName;
            _useFT = useFT;
            _isInsert = isInsert;
            _keysValues = keysValues;
            _additionalButtons = additionalButtons;
            await App.OpenFunction(editDataPage, tiles, Width - menu.ActualWidth);
        }

        private async Task TablePageLoad(string tableName, bool readOnly, bool editable, bool savePdf, DataTable t = null, RoutedEventHandler editClick = null)
        {
            _columnsTypes.Clear();
            _tableName = tableName;

            List<string> columns = new List<string>();
            if (t == null)
            {
                DataTable c = new DataTable();
                switch(App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            c = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_columns_table '{tableName}', 1", App.SystemConfigs.ConnectionStr));
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            break;
                        }
                }

                for (int i = 0; i < c.Rows.Count; i++)
                    c.Rows.Cast<DataRow>().Where(r => !Equals(r, c.Rows[i]) && string.Equals(r[0].ToString(), c.Rows[i][0].ToString())).ToList().ForEach((DataRow r) => c.Rows.Remove(r));

                _columnsTypes.Add(tableName, new Columns());
                c.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                {
                    columns.Add(r[0].ToString());
                    if (r[3] != null)
                        _columnsTypes[tableName].PrimaryColumns.Add(r[0].ToString());
                    else if (string.Equals((string)r[1], "image") || r[1].ToString().Contains("binary"))
                        _columnsTypes[tableName].BinaryColumns.Add(r[0].ToString());
                });
            }
            else
                t.Columns.Cast<DataColumn>().ToList().ForEach((DataColumn c) => columns.Add(c.ColumnName));
            await App.OpenFunction(table, tiles, Width - menu.ActualWidth, new Action<Grid, string, string[], bool, bool, bool, DataTable, RoutedEventHandler>(GenerateTable), new object[] { tablePage, tableName, columns.ToArray(), readOnly, editable, savePdf, t, editClick });
        }

        //Открытие файла из бд
        private void OpenFile(object sender, MouseEventArgs e) => _binaryDataControls.Where(kvp => Equals(kvp.Key, sender)).ToList().ForEach(async (KeyValuePair<UIElement, byte[]> kvp) => await OpeningFile(kvp.Value));
        private async Task OpeningFile(byte[] data)
        {
            string[] filesNames = Directory.GetFiles(new DirectoryInfo(".").FullName).Where(fn => fn.Contains(_cacheFileName)).ToArray();

            int length = BitConverter.ToInt32(data.Take(4).ToArray(), 0) + 4;
            string cacheFileName = $"{_cacheFileName}{filesNames.Length}.{_filesTypes[BitConverter.ToInt32(new byte[4] { data[length], data[length + 1], data[length + 2], data[length + 3] }, 0)]}";

            using (BinaryWriter bw = new BinaryWriter(File.Open(cacheFileName, FileMode.CreateNew)))
                bw.Write(data, length + 4, data.Length - (length + 4));

            Process process = Process.Start(cacheFileName);
            _openedFiles.Add(process);
            for (int i = 0; i < (await Task.Run(() => Process.GetProcesses())).Length; i++)
                i = (await Task.Run(() => Process.GetProcesses().Where(p => string.Equals(p.ProcessName, process.ProcessName)).ToArray())).Length > 0 ? 0 : i;

            _openedFiles.Remove(process);
            File.Delete(cacheFileName);
        }

        private void DeleteRow<T>(Grid container, T conn) where T : DbConnection, new() => _columnsTypes.Where(k => !Equals(k, _tableName) && _rowsOfTables.ContainsKey(k.Key)).ToList().ForEach((KeyValuePair<string, Columns> kvp) =>
        {
            string where = null;
            _rowsOfTables[kvp.Key].Where(uie => !container.Children.Contains((UIElement)uie.Key.GetParentObject())).ToList().ForEach((KeyValuePair<UIElement, string> uie) =>
            {
                string colName = uie.Key.GetType().GetProperty("ToolTip").GetValue(uie.Key).ToString(), value;

                if (uie.Key is TextBox || uie.Key is TextBlock || uie.Key is ComboBox)
                {
                    if (string.Equals(uie.Key.GetType().GetProperty("Text").GetValue(uie.Key).ToString(), "<Открыть файл>"))
                        return;
                    else
                        value = uie.Key.GetType().GetProperty("Text").GetValue(uie.Key).ToString();
                }
                else if (uie.Key is DateTimePicker || uie.Key is TimePicker)
                    value = uie.Key.GetType().GetProperty("SelectedDateTime").GetValue(uie.Key).ToString();
                else
                    value = uie.Key.GetType().GetProperty("SelectedDate").GetValue(uie.Key).ToString();

                value = string.IsNullOrEmpty(value) ? "NULL" : value;
                if (_editable)
                    where += kvp.Value.StrColumns.Contains(colName) || kvp.Value.DateTimeColumns.Contains(colName) || kvp.Value.DateColumns.Contains(colName) || kvp.Value.TimeColumns.Contains(colName) ? $"{uie.Key.GetType().GetProperty("ToolTip").GetValue(uie.Key)} = '{value}' and " : $"{uie.Key.GetType().GetProperty("ToolTip").GetValue(uie.Key)} = {value} and ";
            });

            if (where != null)
            {
                where = where.Substring(0, where.Length - 4);
                Query(new SqlCommand($"DELETE FROM {kvp.Key} WHERE {where}", conn as SqlConnection));
            }
        });

        private void LoadTiles<TContainer, TDataAdapter>(string tableName, int indexOfColumn, double minW, double maxW, int height, bool randomW, TDataAdapter ad, MouseButtonEventHandler mbeh) where TContainer : ContentControl, new() where TDataAdapter : DbDataAdapter, new()
        {
            tiles.Items.Clear();
            _tilesCollection.Clear();

            DataTable t = new DataTable();
            try
            {
                if (maxW < minW)
                    throw new Exception("Максимальная ширина плитки не может быть меньше минимальной.");

                switch (App.SystemConfigs.SelectedDBMS)
                {
                    case DBMS.MSSQL:
                        {
                            t = QuerySelect<SqlDataAdapter, DataTable>(ad as SqlDataAdapter);
                            break;
                        }
                    case DBMS.MySQL:
                        {
                            t = QuerySelect<MySqlDataAdapter, DataTable>(ad as MySqlDataAdapter);
                            break;
                        }
                }

                Random random = new Random();
                t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => _tilesCollection.Add(new TContainer
                {
                    Width = randomW && (!double.IsNaN(minW) || !double.IsNaN(maxW)) ? random.Next((int)minW, (int)maxW) : maxW,
                    Height = height,
                    Padding = new Thickness(20),
                    Content = r[indexOfColumn]
                }, mbeh));

                _tilesCollection.ToList().ForEach((KeyValuePair<ContentControl, MouseButtonEventHandler> a) =>
                {
                    tiles.Items.Add(a.Key);
                    a.Key.MouseDown += a.Value;
                    a.Key.AddHandler(MouseDownEvent, a.Value, true);
                });
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            }
        }
    }
}