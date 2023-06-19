using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;

namespace GasStation
{
    public partial class MainWindow
    {
        private void TanksInfo_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            addTank.Visibility = App.SystemConfigs.Login.Contains("tanker") ? Visibility.Hidden : Visibility.Visible;
            typesList.Visibility = App.SystemConfigs.Login.Contains("tanker") ? Visibility.Hidden : Visibility.Visible;

            if (tanksInfo.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable t = new DataTable();
                    switch (App.SystemConfigs.SelectedDBMS)
                    {
                        case DBMS.MSSQL:
                            {
                                t = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_tanks_volume", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                        case DBMS.MySQL:
                            {
                                t = QuerySelect<MySqlDataAdapter, DataTable>(new MySqlDataAdapter("exec get_tanks_volume", App.SystemConfigs.ConnectionStr));
                                break;
                            }
                    }

                    tanksInfoContent.Children.Clear();
                    tanksInfoContent.RowDefinitions.Clear();
                    tanksInfoContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });

                    int i = 0;
                    t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                    {
                        if (i == 2)
                        {
                            tanksInfoContent.RowDefinitions.Add(new RowDefinition { Height = new GridLength(300) });
                            i = 0;
                        }

                        tanksInfoContent.Children.Add(new Grid
                        {
                            Margin = new Thickness(5),
                            RenderTransformOrigin = new Point(0.5, 0.5),
                        });

                        ((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children.Add(new Label
                        {
                            Content = r[0],
                            Margin = new Thickness(5),
                            FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                            FontSize = 25,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        });

                        double val = 0;
                        if (string.IsNullOrEmpty(r[2].ToString()))
                        {
                            DataTable comings = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_comings '{r[0]}'", App.SystemConfigs.ConnectionStr));
                            if (comings.Rows.Count > 0)
                                val = double.Parse(comings.Rows[0][1].ToString());
                            else
                                val = 0;
                        }
                        else
                            val = double.Parse(r[2].ToString());

                        ((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children.Add(new ProgressBar
                        {
                            Maximum = double.Parse(r[1].ToString().Replace('.', ',')),
                            Value = val,
                            Margin = new Thickness(0, 0, 0, 66),
                            FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                            FontSize = 25,
                            Width = 200,
                            Height = 100,
                            RenderTransformOrigin = new Point(0.5, 0.5),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Style = (Style)Application.Current.Resources["pbStyle"],
                            RenderTransform = new TransformGroup()
                        });

                        ((TransformGroup)((ProgressBar)((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children[1]).RenderTransform).Children.Add(new ScaleTransform());
                        ((TransformGroup)((ProgressBar)((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children[1]).RenderTransform).Children.Add(new SkewTransform());
                        ((TransformGroup)((ProgressBar)((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children[1]).RenderTransform).Children.Add(new RotateTransform { Angle = 270 });
                        ((TransformGroup)((ProgressBar)((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children[1]).RenderTransform).Children.Add(new TranslateTransform());

                        Grid.SetRow(tanksInfoContent.Children[tanksInfoContent.Children.Count - 1], tanksInfoContent.RowDefinitions.Count - 1);
                        Grid.SetColumn(tanksInfoContent.Children[tanksInfoContent.Children.Count - 1], i);

                        ((Grid)tanksInfoContent.Children[tanksInfoContent.Children.Count - 1]).Children.Add(new Label
                        {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            FontSize = 20,
                            Margin = new Thickness(150 , 100, 0 , 0),
                            FontFamily = new FontFamily("./Resources/Fonts/Gilroy/#Gilroy Medium"),
                            Content = $"Объем: {val.ToString("0.00")}/{r[1]}"
                        });

                        Grid.SetRow(tanksInfoContent.Children[tanksInfoContent.Children.Count - 1], tanksInfoContent.RowDefinitions.Count - 1);
                        Grid.SetColumn(tanksInfoContent.Children[tanksInfoContent.Children.Count - 1], i);

                        i++;
                    });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
        }

        private async void AddTank_Click(object sender, RoutedEventArgs e) => await EditPageLoad("fuel_types", new List<string> { }, false, true, null, new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) => await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth)) } });
        private async void RefuelTank_Click(object sender, RoutedEventArgs e)
        {
            _gasColumnNum = -1;
            await App.OpenFunction(fueling, menu, Width - menu.ActualWidth);
        }
        private async void TableOfComings_Click(object sender, RoutedEventArgs e) => await TablePageLoad("fuel_coming", true, false, true, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_fuel_comings", App.SystemConfigs.ConnectionStr)), null, new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) => await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth)) } });
        private async void TypesList_Click(object sender, RoutedEventArgs e) => await TablePageLoad("fuel_types", true, true, false, QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter("exec get_tanks_volume", App.SystemConfigs.ConnectionStr)), null, new Dictionary<string, RoutedEventHandler> { { "back_arrow", new RoutedEventHandler(async (object obj, RoutedEventArgs rea) => await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth)) } });
    }
}