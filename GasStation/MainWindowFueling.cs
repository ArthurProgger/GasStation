using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.Data;
using System.Data.SqlClient;

namespace GasStation
{
    public partial class MainWindow
    {
        private decimal _selectedFuelPrice;
        private List<decimal> _fuelPrices = new List<decimal>();
        private void Fueling_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (fueling.Visibility == Visibility.Visible)
            {
                try
                {
                    DataTable t = _gasColumnNum == -1 ? QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_tanks_volume", App.SystemConfigs.ConnectionStr)) : QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_fuel_types_of_columns {_gasColumnNum}", App.SystemConfigs.ConnectionStr));
                    int i = _gasColumnNum == -1 ? 0 : 1;

                    if (_gasColumnNum == -1)
                    {
                        fuelSummary.Visibility = Visibility.Hidden;
                        fuelSummaryL.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        fuelSummary.Visibility = Visibility.Visible;
                        fuelSummaryL.Visibility = Visibility.Visible;
                    }

                    t.Rows.Cast<DataRow>().ToList().ForEach((DataRow r) =>
                    {
                        Border border = new Border
                        {
                            CornerRadius = new CornerRadius(10),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Width = 20,
                            Height = 20
                        };
                        switch ((string)r[i])
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

                        fuelTypes.Items.Add(new ListViewItem
                        {
                            Content = new Grid(),
                            Background = null
                        });

                        if (_gasColumnNum != -1)
                            _fuelPrices.Add((decimal)r[2]);

                        ((Grid)((ListViewItem)fuelTypes.Items[fuelTypes.Items.Count - 1]).Content).Children.Add(border);
                        ((Grid)((ListViewItem)fuelTypes.Items[fuelTypes.Items.Count - 1]).Content).Children.Add(new RadioButton
                        {
                            Margin = new Thickness(30, 0, 0, 0),
                            Content = (string)r[i],
                            Background = null,
                            GroupName = "g"
                        });
                        ((RadioButton)((Grid)((ListViewItem)fuelTypes.Items[fuelTypes.Items.Count - 1]).Content).Children[1]).Checked += new RoutedEventHandler((object obj, RoutedEventArgs rea) =>
                        {
                            fuelVolumeSlider.Value = 0;
                            fuelVolume.Value = 0;

                            DataTable tankVolume = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_tanks_volume '{((RadioButton)obj).Content}'", App.SystemConfigs.ConnectionStr));

                            if (_gasColumnNum == -1)
                            {
                                if (string.IsNullOrEmpty(tankVolume.Rows[0][2].ToString()))
                                {
                                    DataTable volume = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_comings '{((RadioButton)obj).Content}'", App.SystemConfigs.ConnectionStr));
                                    if (volume.Rows.Count < 1)
                                    {
                                        fuelVolumeSlider.Maximum = double.Parse(tankVolume.Rows[0][1].ToString());
                                        fuelVolume.Maximum = double.Parse(tankVolume.Rows[0][1].ToString());
                                    }
                                    else
                                    {
                                        fuelVolumeSlider.Maximum = double.Parse(tankVolume.Rows[0][1].ToString()) - double.Parse(volume.Rows[0][1].ToString());
                                        fuelVolume.Maximum = double.Parse(tankVolume.Rows[0][1].ToString()) - double.Parse(volume.Rows[0][1].ToString());
                                    }
                                }
                                else
                                {
                                    fuelVolumeSlider.Maximum = double.Parse(tankVolume.Rows[0][1].ToString()) - double.Parse(tankVolume.Rows[0][2].ToString());
                                    fuelVolume.Maximum = double.Parse(tankVolume.Rows[0][1].ToString()) - double.Parse(tankVolume.Rows[0][2].ToString());
                                }
                            }
                            else if (string.IsNullOrEmpty(tankVolume.Rows[0][2].ToString()))
                            {
                                DataTable volume = QuerySelect<SqlDataAdapter, DataTable>(new SqlDataAdapter($"exec get_comings '{((RadioButton)obj).Content}'", App.SystemConfigs.ConnectionStr));
                                if (volume.Rows.Count < 1)
                                {
                                    SideMessage.Show(Content as Grid, "Нет топлива!", SideMessage.Type.Error, Position.Right);
                                    ((RadioButton)obj).IsChecked = false;
                                }
                                else
                                {
                                    fuelVolumeSlider.Maximum = double.Parse(volume.Rows[0][1].ToString());
                                    fuelVolume.Maximum = double.Parse(volume.Rows[0][1].ToString());
                                    _selectedFuelPrice = _fuelPrices[fuelTypes.Items.IndexOf((ListViewItem)((RadioButton)obj).Parent.GetParentObject())];
                                }
                            }
                            else
                            {
                                fuelVolumeSlider.Maximum = double.Parse(tankVolume.Rows[0][2].ToString());
                                fuelVolume.Maximum = double.Parse(tankVolume.Rows[0][2].ToString());
                                _selectedFuelPrice = _fuelPrices[fuelTypes.Items.IndexOf(((Grid)((RadioButton)obj).Parent).Parent)];
                            }
                        });
                    });
                }
                catch (Exception err)
                {
                    SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
                }
            }
            else
            {
                _fuelPrices.Clear();
                fuelTypes.Items.Clear();
            }
        }

        private void FuelVolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fuelVolume.Value = fuelVolumeSlider.Value;
            if (_fuelPrices.Count > 0)
                fuelSummary.Value = fuelVolume.Value * (double)_selectedFuelPrice;
        }
        private void FuelVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e) => fuelVolumeSlider.Value = fuelVolume.Value ?? 0;

        private async void SaveFueling_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedFuelType = null;
                fuelTypes.Items.Cast<ListViewItem>().ToList().ForEach((ListViewItem lvi) =>
                {
                    if (((RadioButton)((Grid)lvi.Content).Children[1]).IsChecked.Value)
                        selectedFuelType = ((RadioButton)((Grid)lvi.Content).Children[1]).Content.ToString();
                });

                if (string.IsNullOrEmpty(selectedFuelType))
                    throw new Exception("Не выбран тип топлива!");
                else if (fuelVolumeSlider.Value == 0)
                    throw new Exception("Не указано количество бензина!");

                if (_gasColumnNum == -1)
                {
                    Query(new SqlCommand($"INSERT INTO fuel_coming VALUES ('{DateTime.Now}', '{selectedFuelType}', {fuelVolumeSlider.Value.ToString().Replace(',', '.')})", GetConnectionObj<SqlConnection>()));
                    await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth);
                    SideMessage.Show(Content as Grid, "Резервуар успешно заправлен!", SideMessage.Type.Info, Position.Right);
                }
                else
                {
                    Query(new SqlCommand($"INSERT INTO fuel_using VALUES ('{DateTime.Now}', '{selectedFuelType}', {fuelVolumeSlider.Value.ToString().Replace(',', '.')}, {_gasColumnNum})", GetConnectionObj<SqlConnection>()));
                    await App.OpenFunction(gasColumnsInfo, tiles, Width - menu.ActualWidth);
                    SideMessage.Show(Content as Grid, "Заправка успешно проведена!", SideMessage.Type.Info, Position.Right);
                }
            }
            catch (Exception err)
            {
                SideMessage.Show(Content as Grid, err.Message, SideMessage.Type.Error, Position.Right);
            }
        }

        private async void FuelingBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_gasColumnNum == -1)
                await App.OpenFunction(tanksInfo, tiles, Width - menu.ActualWidth);
            else
                await App.OpenFunction(gasColumnsInfo, tiles, Width - menu.ActualWidth);
        }
    }
}