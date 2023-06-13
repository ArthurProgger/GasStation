using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;

namespace GasStation
{
    public partial class MainWindow
    {
        private void GasColumnsList_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (gasColumnsInfo.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable t = new DataTable();

                    switch (App.SystemConfigs.SelectedDBMS)
                    {
                        case DBMS.MSSQL:
                            {
                                t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_fuel_types_of_columns", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                        case DBMS.MySQL:
                            {
                                t = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter("call get_fuel_types_of_columns", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                    }

                    int i = 0;
                    List<int> nums = new List<int>(); 
                        
                    t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                    {
                        if (i == 2)
                        {
                            gasColumnsList.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
                            i = 0;
                        }
                        else if (!nums.Contains((int)r[0]))
                        {
                            gasColumnsList.Children.Add(new Grid());

                            ((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children.Add(new Label
                            {
                                Margin = new Thickness(5),
                                FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                                FontSize = 25,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                                Content = $"Колонка №{r[0]}"
                            });

                            nums.Add((int)r[0]);                            

                            ((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children.Add(new Button
                            {
                                Content = new Image
                                {
                                    Source = Imaging.CreateBitmapSourceFromHBitmap(
                                               Properties.Resources.gas_station_icon.GetHbitmap(),
                                               IntPtr.Zero,
                                               Int32Rect.Empty,
                                               BitmapSizeOptions.FromEmptyOptions())
                                },
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Width = 30,
                                Height = 30
                            });
                            ((Button)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[1]).Click += new RoutedEventHandler(async (object obj, RoutedEventArgs rea) =>
                            {
                                _gasColumnNum = (int)r[0];
                                await App.OpenFunction(fueling, menu, Width - menu.ActualWidth);
                            });

                            ((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children.Add(new ListView
                            {
                                Margin = new Thickness(5, 49, 0, 5),
                                Background = null,
                                HorizontalAlignment = HorizontalAlignment.Left
                            });

                            t.Rows.Cast<DataRow>().Where(toc => toc[0].ToString() == r[0].ToString()).ToList().ForEach((DataRow ct) =>
                            {
                                Border border = new Border
                                {
                                    CornerRadius = new CornerRadius(10),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    Width = 20,
                                    Height = 20
                                };
                                switch ((string)ct[1])
                                {
                                    case "АИ-98":
                                        {
                                            border.Background = new SolidColorBrush(Colors.Green);
                                            break;
                                        }
                                    case "АИ-95":
                                        {
                                            border.Background = new SolidColorBrush(Colors.Red);
                                            break;
                                        }
                                    case "АИ-92":
                                        {
                                            border.Background = new SolidColorBrush(Colors.Blue);
                                            break;
                                        }
                                    case "ДТ":
                                        {
                                            border.Background = new SolidColorBrush(Colors.Black);
                                            break;
                                        }
                                }

                                ((ListView)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[2]).Items.Add(new ListViewItem
                                {
                                    Padding = new Thickness(5),
                                    Style = (Style)Application.Current.Resources["lviStyle"],
                                    Cursor = Cursors.Hand,
                                    Content = new Grid()
                                });

                                ((Grid)((ListViewItem)((ListView)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[2]).Items[((ListView)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[2]).Items.Count - 1]).Content).Children.Add(border);
                                ((Grid)((ListViewItem)((ListView)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[2]).Items[((ListView)((Grid)gasColumnsList.Children[gasColumnsList.Children.Count - 1]).Children[2]).Items.Count - 1]).Content).Children.Add(new Label
                                {
                                    FontSize = 14,
                                    FontFamily = new System.Windows.Media.FontFamily("./Resources/Fonts/Gilroy/#Gilroy Light Italic"),
                                    Padding = new Thickness(25, 0, 0, 0),
                                    VerticalContentAlignment = VerticalAlignment.Center,
                                    HorizontalAlignment = HorizontalAlignment.Right,
                                    Margin = new Thickness(0, 0, -25, 0),
                                    Width = 100,
                                    Content = ct[1]
                                });
                            });

                            Grid.SetColumn(gasColumnsList.Children[gasColumnsList.Children.Count - 1], i);
                            Grid.SetRow(gasColumnsList.Children[gasColumnsList.Children.Count - 1], gasColumnsList.RowDefinitions.Count - 1);

                            i++;
                        }
                    });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else
            {
                gasColumnsList.Children.Clear();
                gasColumnsList.RowDefinitions.Where(rd => gasColumnsList.RowDefinitions.IndexOf(rd) > 0).ToList().ForEach((RowDefinition rd) => gasColumnsList.RowDefinitions.Remove(rd));
            }
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e) => await EditPageLoad("gas_columns", new List<string> { }, false, true);
        private async void TableOfGasColumns_Click(object sender, RoutedEventArgs e) => await TablePageLoad("gas_columns", true, true, false, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec [get_gas_columns]", App.SystemConfigs.ConnectionStr)));
        private async void FuelingGasColumns_Click(object sender, RoutedEventArgs e) => await TablePageLoad("fuel_using", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec [get_fuelings]", App.SystemConfigs.ConnectionStr)));
    }
}