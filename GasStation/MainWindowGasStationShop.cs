using System;
using System.IO;
using System.Diagnostics;
using SWF = System.Windows.Forms;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private void LoadTilesOfGasStationShop<T>() where T : ContentControl, new()
        {
            tiles.Items.Clear();
            _tilesCollection.Clear();

            T tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Добавить товар",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.add_product.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Добавить товар",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await EditPageLoad("products", new List<string>(), false, true, null, new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
            {
                                    App.SystemConfigs.ConnectionStr = "GasStationShop";
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
            }) } })));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список товаров",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.product_range.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Список товаров",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await TablePageLoad("products", true, true, false, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("SELECT * FROM products", App.SystemConfigs.ConnectionStr)), null, new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
                }) },
                {
                    "add_product" , new RoutedEventHandler(async (object obj, RoutedEventArgs rea) => await EditPageLoad("products_types", new List<string>(), false, true))
                } })));//get_products

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список поставщиков",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.team.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Список поставщиков",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                try
                {
                    DataTable t = new DataTable();
                    switch (App.SystemConfigs.SelectedDBMS)
                    {
                        case DBMS.MSSQL:
                            {
                                t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_suppliers", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                        case DBMS.MySQL:
                            {
                                t = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter("exec get_suppliers", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                    }
                    await TablePageLoad("suppliers", true, true, false, t, new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                    {
                        Grid container = (Grid)((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent).Parent;
                        string tableName = tablesPieces.Items.Count > 0 ? ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString() : _tableName;
                        tableName = _tableSynonyms.ContainsKey(tableName) ? tableName : _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToArray()[0].Key;
                        List<string> values = new List<string>();

                        int rIndex = Grid.GetRow((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent);
                        container.Children.Cast<UIElement>().Where(c => !(((Border)c).Child is Label) && Grid.GetRow(c) == rIndex).ToList().ForEach((UIElement c) =>
                        {
                            if (_supplierId == -1)
                                _supplierId = int.Parse(((TextBlock)((Border)c).Child).Text);
                        });

                        await App.OpenFunction(companiesPage, tiles, Width - menu.ActualWidth);
                    }), new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
                }) },
                    { "write", new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                    {
                        // Шаблон договора с поставщиками

                        SWF.SaveFileDialog sfd = new SWF.SaveFileDialog
                        {
                            Filter = "Документ Word|*.docx"
                        };
                        sfd.ShowDialog();
                        if (!string.IsNullOrEmpty(sfd.FileName))
                        {
                            using (BinaryWriter bw = new BinaryWriter(File.Open(sfd.FileName, FileMode.CreateNew)))
                                bw.Write(Properties.Resources.Шаблон_договора_с_поставщиками, 0 , Properties.Resources.Шаблон_трудового_договора.Length);

                            Process.Start(sfd.FileName);
                        }
                    }) } });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Новый поставщик",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.supp_add.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Новый поставщик",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });

            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                await App.OpenFunction(companiesPage, tiles, Width - menu.ActualWidth);
            }));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список поставок",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.truck.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Список поставок",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                try
                {
                    await TablePageLoad("products_supplies", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_supplies", App.SystemConfigs.ConnectionStr)), null, new Dictionary<string, RoutedEventHandler> { { "back_arrow.png", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
                }) } });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Сформировать корзину",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.shopping_basket.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Сформировать корзину",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                _isSale = true;
                await App.OpenFunction(basket, tiles, Width - menu.ActualWidth);
            }));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Новая поставка",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.add_supply.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Новая поставка",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                _isSale = false;
                await App.OpenFunction(basket, tiles, Width - menu.ActualWidth);
            }));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Продажи",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.sale.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Продажи",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await TablePageLoad("sales", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_sales", App.SystemConfigs.ConnectionStr)), new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
            {
                Grid container = (Grid)((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent).Parent;
                string tableName = tablesPieces.Items.Count > 0 ? ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString() : _tableName;
                tableName = _tableSynonyms.ContainsKey(tableName) ? tableName : _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToArray()[0].Key;
                List<string> values = new List<string>();

                int rIndex = Grid.GetRow((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent);
                container.Children.Cast<UIElement>().Where(c => !(((Border)c).Child is Label) && Grid.GetRow(c) == rIndex).ToList().ForEach((UIElement c) =>
                {
                    if (_supplierId == -1)
                        _supplierId = int.Parse(((TextBlock)((Border)c).Child).Text);
                });

                await App.OpenFunction(companiesPage, tiles, Width - menu.ActualWidth);
            }), new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));

            }) } })));

            tile = new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Отчет по остаткам",
                Content = new Grid(),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            };
            ((Grid)tile.Content).Children.Add(new Image
            {
                Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.count.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(15)
            });
            ((Grid)tile.Content).Children.Add(new Label
            {
                Content = "Отчет по остаткам",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                FontSize = 20
            });
            _tilesCollection.Add(tile, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                DataTable products = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_products", App.SystemConfigs.ConnectionStr)),
                counts = new DataTable();
                counts.Columns.Add("Название", typeof(string));
                counts.Columns.Add("Остаток", typeof(int));

                products.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => counts.Rows.Add(new object[] { r[1], QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_prod_count {r[0]}", App.SystemConfigs.ConnectionStr)).Rows[0][1] }));

                await TablePageLoad("products", true, false, true, counts, new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    Grid container = (Grid)((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent).Parent;
                    string tableName = tablesPieces.Items.Count > 0 ? ((TabItem)tablesPieces.Items[tablesPieces.SelectedIndex]).Header.ToString() : _tableName;
                    tableName = _tableSynonyms.ContainsKey(tableName) ? tableName : _tableSynonyms.Where(kvp => string.Equals(kvp.Value.TableSynonym, tableName)).ToArray()[0].Key;
                    List<string> values = new List<string>();

                    int rIndex = Grid.GetRow((Border)((Label)((Grid)((Button)obj).Parent).Parent).Parent);
                    container.Children.Cast<UIElement>().Where(c => !(((Border)c).Child is Label) && Grid.GetRow(c) == rIndex).ToList().ForEach((UIElement c) =>
                    {
                        if (_supplierId == -1)
                            _supplierId = int.Parse(((TextBlock)((Border)c).Child).Text);
                    });

                    await App.OpenFunction(companiesPage, tiles, Width - menu.ActualWidth);
                }), new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                {
                    if (App.SystemConfigs.UseWin8TilesStyle)
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
                    else
                        await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));

            }) } });
            }));

            _tilesCollection.ToList().ForEach((KeyValuePair<ContentControl, MouseButtonEventHandler> a) =>
            {
                tiles.Items.Add(a.Key);
                a.Key.MouseDown += a.Value;
                a.Key.AddHandler(MouseDownEvent, a.Value, true);
            });
        }

        private void ShopMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}