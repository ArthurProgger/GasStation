using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private bool _isSale;

        private void Basket_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (basket.Visibility == Visibility.Visible)
            {
                //salerName.Content = $"Продавец: {_userFullName}";
                try
                {
                    DataTable t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_products", App.SystemConfigs.ConnectionStr));

                    _tableName = "products";
                    GenerateTable(productsSales, _tableName, new string[0], true, false, false, t);

                    BuyColumn();

                    basketSuppliersL.Visibility = _isSale ? Visibility.Hidden : Visibility.Visible;
                    basketSuppliers.Visibility = _isSale ? Visibility.Hidden : Visibility.Visible;
                    if (!_isSale)
                    {
                        t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_suppliers", App.SystemConfigs.ConnectionStr));
                        t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) => basketSuppliers.Items.Add($"{r[0]}, {r[1]}"));
                    }
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else
            {
                productsSales.Children.Clear();
                productsSales.RowDefinitions.Clear();
                productsSales.ColumnDefinitions.Clear();
                basketContent.Items.Clear();
                basketSuppliers.Items.Clear();
                _columnsTypes.Clear();
            }
        }

        private void SearchProductsSales_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTables(productsSales, productsSalesSV, searchProductsSales);
            BuyColumn();
        }
        private void SalesColumns()
        {
            productsSales.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

            Border b = new Border
            {
                Background = new SolidColorBrush(App.SystemConfigs.FunctionsColor),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2, 2, 2, 0),
                Child = new Label
                {
                    Content = "Продать"
                }
            };
            Grid.SetColumn(b, productsSales.ColumnDefinitions.Count - 1);

            ScrollViewer sv = (ScrollViewer)productsSales.Children[productsSales.Children.Count - 1];

            b.CornerRadius = ((Border)productsSales.Children[productsSales.Children.Count - 2]).CornerRadius;
            ((Border)productsSales.Children[productsSales.Children.Count - 2]).CornerRadius = new CornerRadius(0);
            productsSales.Children.Remove(sv);
            productsSales.Children.Add(b);
            productsSales.Children.Add(sv);

            Grid.SetColumnSpan(sv, 4);
            ((Grid)sv.Content).ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

            Grid svContent = (Grid)sv.Content;

            int i = 0;
            _dataTable.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
            {
                //if (_productsBasket.)

                b = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(2, 2, 2, 0),
                    Child = new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                };

                svContent.Children.Add(b);
                Grid.SetRow(b, i);
                Grid.SetColumn(b, svContent.ColumnDefinitions.Count - 1);

                i++;
            });
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (basketContent.Items.Count > 0)
                {
                    if (_isSale)
                    {
                        Query(new SqlCommand($"INSERT INTO sales VALUES ('{DateTime.Now}', NULL)", GetConnectionObj<SqlConnection>()));
                        int id = int.Parse(QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_last_index_of_sale", App.SystemConfigs.ConnectionStr)).Rows[0][0].ToString());

                        basketContent.Items.Cast<ListViewItem>().ToList().ForEach((ListViewItem lvi) =>
                        {
                            int prodId = int.Parse(((Label)((Grid)lvi.Content).Children[0]).Content.ToString().Substring(0, ((Label)((Grid)lvi.Content).Children[0]).Content.ToString().IndexOf(',')));
                            int prodCount = (int)((NumericUpDown)((Grid)lvi.Content).Children[1]).Value.Value;
                            Query(new SqlCommand($"INSERT INTO sales_content VALUES ({id}, {prodId}, {prodCount})", GetConnectionObj<SqlConnection>()));
                        });
                        SideMessage.Show(Content as Grid, "Товары проданы!", SideMessage.Type.Info, Position.Right);
                    }
                    else
                    {
                        int suppId = int.Parse(basketSuppliers.SelectedItem.ToString().Substring(0, basketSuppliers.SelectedItem.ToString().IndexOf(',')));
                        Query(new SqlCommand($"INSERT INTO supplies VALUES ('{DateTime.Now}', {suppId})", GetConnectionObj<SqlConnection>()));
                        int id = int.Parse(QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_last_index_of_supplies", App.SystemConfigs.ConnectionStr)).Rows[0][0].ToString());

                        basketContent.Items.Cast<ListViewItem>().ToList().ForEach((ListViewItem lvi) =>
                        {
                            int prodId = int.Parse(((Label)((Grid)lvi.Content).Children[0]).Content.ToString().Substring(0, ((Label)((Grid)lvi.Content).Children[0]).Content.ToString().IndexOf(',')));
                            int prodCount = (int)((NumericUpDown)((Grid)lvi.Content).Children[1]).Value.Value;
                            Query(new SqlCommand($"INSERT INTO supplies_content VALUES ({id}, {prodId}, {prodCount})", GetConnectionObj<SqlConnection>()));
                        });
                        SideMessage.Show(Content as Grid, "Товары пришли на склад!", SideMessage.Type.Info, Position.Right);
                    }

                    menu.SelectedIndex = 1;
                }
                else
                    throw new Exception("Список товаров пуст!");
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            }
        }

        private void BuyColumn()
        {
            productsSales.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

            Border b = new Border
            {
                Background = new SolidColorBrush(App.SystemConfigs.FunctionsColor),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2, 2, 2, 0),
                Child = new Label
                {
                    Content = "Продать"
                }
            };
            Grid.SetColumn(b, productsSales.ColumnDefinitions.Count - 1);

            ScrollViewer sv = (ScrollViewer)productsSales.Children[productsSales.Children.Count - 1];

            b.CornerRadius = ((Border)productsSales.Children[productsSales.Children.Count - 2]).CornerRadius;
            ((Border)productsSales.Children[productsSales.Children.Count - 2]).CornerRadius = new CornerRadius(0);
            productsSales.Children.Remove(sv);
            productsSales.Children.Add(b);
            productsSales.Children.Add(sv);

            Grid.SetColumnSpan(sv, 4);
            ((Grid)sv.Content).ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });

            Grid svContent = (Grid)sv.Content;

            filterTables.Items.Clear();

            filterTables.Items.Cast<CheckBox>().ToList().ForEach((CheckBox cb) =>
            {
                CheckBox cb1 = new CheckBox
                {
                    Content = cb.Content
                };
                cb1.Checked += new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                {
                    SearchTables(productsSales, productsSalesSV, searchProductsSales);
                    BuyColumn();
                });
                cb1.Unchecked += new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                {
                    SearchTables(productsSales, productsSalesSV, searchProductsSales);
                    BuyColumn();
                });
                filterProductsSales.Items.Add(cb1);
            });

            int i = 0;
            svContent.RowDefinitions.Cast<RowDefinition>().ToList().ForEach((RowDefinition r) =>
            {
                Border b1 = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(2, 2, 2, 0),
                    Child = new Button
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 30,
                        Height = 30,
                        Content = new Image
                        {
                            Source = Imaging.CreateBitmapSourceFromHBitmap(
                                       Properties.Resources.shopping_basket.GetHbitmap(),
                                       IntPtr.Zero,
                                       Int32Rect.Empty,
                                       BitmapSizeOptions.FromEmptyOptions()),
                            Stretch = Stretch.Uniform
                        }
                    }
                };
                ((Button)b1.Child).Click += new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                {
                    var row = ((Grid)b1.Parent).Children.Cast<Border>().Where(bord => Grid.GetRow(bord) == Grid.GetRow(b1));
                    ListViewItem lvi = new ListViewItem
                    {
                        Content = new Grid()
                    };

                    if (basketContent.Items.Cast<ListViewItem>().Where(lvi1 => ((Grid)lvi1.Content).Children.Count > 0).Where(lvi1 => ((Label)((Grid)lvi1.Content).Children[0]).Content.ToString() == $"{((TextBlock)row.ToArray()[0].Child).Text}, {((TextBlock)row.ToArray()[1].Child).Text}").Count() < 1)
                    {
                        try
                        {
                            int count = -1;
                            if (_isSale)
                            {
                                DataTable t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_prod_count {((TextBlock)row.ToArray()[0].Child).Text}", App.SystemConfigs.ConnectionStr));
                                if ((int)t.Rows[0][0] == 0)
                                    throw new Exception("Товара нет на складе!");
                                else
                                    count = (int)t.Rows[0][0];
                            }

                            basketContent.Items.Add(lvi);

                            ((Grid)((ListViewItem)basketContent.Items[basketContent.Items.Count - 1]).Content).Children.Add(new Label
                            {
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Width = 195,
                                ToolTip = $"{((TextBlock)row.ToArray()[0].Child).Text}, {((TextBlock)row.ToArray()[1].Child).Text}",
                                Content = $"{((TextBlock)row.ToArray()[0].Child).Text}, {((TextBlock)row.ToArray()[1].Child).Text}"
                            });

                            ((Grid)((ListViewItem)basketContent.Items[basketContent.Items.Count - 1]).Content).Children.Add(new NumericUpDown
                            {
                                Margin = new Thickness(200, 0, 31, 0),
                                Minimum = 1,
                                Maximum = count == -1 ? 1000 : count
                            });
                            Button butt = new Button
                            {
                                HorizontalAlignment = HorizontalAlignment.Right,
                                Width = 26,
                                Content = "X"
                            };
                            ((Grid)((ListViewItem)basketContent.Items[basketContent.Items.Count - 1]).Content).Children.Add(butt);
                            butt.Click += new RoutedEventHandler((object obj1, RoutedEventArgs rea1) => basketContent.Items.Remove(lvi));
                        }
                        catch (Exception err)
                        {
                            SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                        }
                    }
                });

                svContent.Children.Add(b1);
                Grid.SetRow(b1, i);
                Grid.SetColumn(b1, svContent.ColumnDefinitions.Count - 1);

                i++;
            });
        }

        private async void BasketBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.SystemConfigs.ConnectionStr = "GasStationShop";
            if (App.SystemConfigs.UseWin8TilesStyle)
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<Tile>));
            else
                await App.OpenFunction(tilesPage, menu, Width - menu.ActualWidth, new Action(LoadTilesOfGasStationShop<ListViewItem>));
        }
    }
}