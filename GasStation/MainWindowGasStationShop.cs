using System;
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

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Добавить товар",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.add_product.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await EditPageLoad("products", new List<string>(), false, true)));

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список товаров",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.product_range.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) => await TablePageLoad("products", true, true, false, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("SELECT * FROM products", App.SystemConfigs.ConnectionStr)))));//get_products

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список поставщиков",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.team.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
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
                    await TablePageLoad("products_suppliers", true, true, false, t, new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                    {
                        _fuelSuppliers = false;

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
                    }));
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }));

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Новый поставщик",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.supp_add.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                _fuelSuppliers = false;
                await App.OpenFunction(companiesPage, tiles, Width - menu.ActualWidth);
            }));

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Список поставок",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.truck.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                try
                {
                    await TablePageLoad("products_supplies", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_supplies", App.SystemConfigs.ConnectionStr)));
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }));

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Сформировать корзину",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.shopping_basket.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                _isSale = true;
                await App.OpenFunction(basket, tiles, Width - menu.ActualWidth);
            }));

            _tilesCollection.Add(new T
            {
                Width = 220,
                Height = 120,
                ToolTip = "Новая поставка",
                Content = new Image
                {
                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.add_supply.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(20)
                },
                FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            }, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            {
                _isSale = false;
                await App.OpenFunction(basket, tiles, Width - menu.ActualWidth);
            }));

            //_tilesCollection.Add(new T
            //{
            //    Width = 220,
            //    Height = 120,
            //    ToolTip = "Список поставок",
            //    Content = new Image
            //    {
            //        Source = Imaging.CreateBitmapSourceFromHBitmap(
            //               Properties.Resources.truck.GetHbitmap(),
            //               IntPtr.Zero,
            //               Int32Rect.Empty,
            //               BitmapSizeOptions.FromEmptyOptions()),
            //        Stretch = Stretch.Uniform,
            //        Margin = new Thickness(20)
            //    },
            //    FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic")
            //}, new MouseButtonEventHandler(async (object sender, MouseButtonEventArgs e) =>
            //{
            //    try
            //    {
            //        await TablePageLoad("products_supplies", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_supplies", App.SystemConfigs.ConnectionStr)));
            //    }
            //    catch (Exception err)
            //    {
            //        SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            //    }
            //}));


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